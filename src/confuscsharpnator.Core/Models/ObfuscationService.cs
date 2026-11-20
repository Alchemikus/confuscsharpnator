using confuscsharpnator.Core.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace confuscsharpnator.Core.Models
{
    /// <summary>
    /// Provides the core implementation for the obfuscation process. 
    /// Handles the lifecycle of a C# solution: loading via MSBuild, applying obfuscation passes, and saving the mutated syntax trees.
    /// </summary>
    /// <remarks>
    /// This service focuses strictly on compilable C# projects (e.g., .sln, .slnx, .csproj, and .cs files). 
    /// Other project resources (like XAML or image files) are not processed or copied by this service.
    /// </remarks>
    public class ObfuscationService : IObfuscationService
    {
        private readonly IFileWriter _fileWriter;
        private readonly IEnumerable<IObfuscatorStep> _csharpSteps;
        private MSBuildWorkspace? _workspace;
        private Solution? _roslynSolution;
        private ObfuscationContext _context;
        private string _solutionRootPath;
        private bool _disposed = false;

        private bool IsInitialized => _roslynSolution != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObfuscationService"/> class.
        /// </summary>
        /// <param name="fileWriter">The service responsible for writing the obfuscated files to the physical file system.</param>
        /// <param name="csharpSteps">The collection of available obfuscation steps (passes) injected via dependency injection.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Użyj konstruktora podstawowego", Justification = "<Oczekujące>")]
        public ObfuscationService(
            IFileWriter fileWriter,
            IEnumerable<IObfuscatorStep> csharpSteps)
        {
            _fileWriter = fileWriter;
            _csharpSteps = csharpSteps;

            _context = new ObfuscationContext();
            _solutionRootPath = string.Empty;
        }

        /// <summary>
        /// Asynchronously loads and parses the target C# solution using MSBuild tools.
        /// This method must be called before attempting to obfuscate or save.
        /// </summary>
        /// <param name="solutionFilePath">The absolute path to the .sln or .slnx file.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="solutionFilePath"/> is null, empty, or points to a non-existent file.</exception>
        public async Task InitializeAsync(string solutionFilePath)
        {
            if (string.IsNullOrWhiteSpace(solutionFilePath) || !File.Exists(solutionFilePath))
                throw new ArgumentException("Invalid solution file path.", nameof(solutionFilePath));
            solutionFilePath = Path.GetFullPath(solutionFilePath);

            // If there exist a previously loaded workspace, dispose it to free resources before loading a new solution.
            _roslynSolution = null;
            _workspace?.Dispose();
            _workspace = null;

            // Store the root path to calculate relative paths later during the save phase.
            _solutionRootPath = Path.GetDirectoryName(solutionFilePath)!;
            _context = new ObfuscationContext();

            // MSBuildWorkspace automatically resolves project dependencies, NuGet packages, 
            // and preprocessor directives to build a complete semantic model.
            _workspace = MSBuildWorkspace.Create();
            _roslynSolution = await _workspace.OpenSolutionAsync(solutionFilePath);

            if (!_roslynSolution.Projects.Any())
            {
                var errors = string.Join(
                    Environment.NewLine,
                    _workspace.Diagnostics
                        .Where(d => d.Kind == WorkspaceDiagnosticKind.Failure)
                        .Select(d => d.Message));

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(errors)
                        ? "Nie załadowano żadnego projektu z pliku rozwiązania."
                        : $"MSBuild nie załadował projektów:{Environment.NewLine}{errors}");
            }
        }

        /// <summary>
        /// Executes the specified obfuscation steps on the currently loaded solution.
        /// </summary>
        /// <param name="selectedStepNames">A collection of obfuscation step names that the user opted to execute.</param>
        /// <exception cref="InvalidOperationException">Thrown when the service has not been initialized via <see cref="InitializeAsync"/>.</exception>
        public void Obfuscate(IEnumerable<string> selectedStepNames)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Service is not initialized. Please call InitializeAsync first.");

            var selectedSet = new HashSet<string>(selectedStepNames, StringComparer.OrdinalIgnoreCase);

            // Filter the injected steps to run only those explicitly requested by the user,
            // ordered by ExecutionOrder (ascending) with a deterministic name tie-breaker.
            var stepsToExecute = _csharpSteps
                .Where(s => selectedSet.Contains(s.Name))
                .OrderBy(s => s.ExecutionOrder)
                .ThenBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var currentSolution = _roslynSolution!;

            foreach (var step in stepsToExecute)
            {
                // Roslyn models are completely immutable. 
                // Each step execution must return a brand new Solution instance containing the applied mutations.
                currentSolution = step.Process(currentSolution, _context);
            }

            _roslynSolution = currentSolution;
        }

        /// <summary>
        /// Asynchronously extracts the mutated C# syntax trees and core structural files (solution, projects) 
        /// from the Roslyn workspace and writes them to the specified output directory.
        /// </summary>
        /// <param name="targetDirectoryPath">The absolute path to the directory where the obfuscated solution should be saved.</param>
        /// <exception cref="InvalidOperationException">Thrown when there is no loaded solution to save.</exception>
        public async Task SaveAsync(string targetDirectoryPath)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("No solution to save. Please call InitializeAsync first.");
            if (string.IsNullOrWhiteSpace(targetDirectoryPath))
                throw new ArgumentException("Target directory path cannot be empty.", nameof(targetDirectoryPath));

            var outputFiles = new Dictionary<string, string>();

            // We use a strict whitelist approach. We only save structural files and mutated C# code.
            // This prevents accidental data leaks of sensitive local files (like secrets.json or .git folders).
            // It also has its disadvantage: any non-C# files (like XAML, images, or other resources) will not be copied to the output folder.

            // 1. Prepare to save the master solution file (.sln or .slnx).
            if (!string.IsNullOrWhiteSpace(_roslynSolution!.FilePath))
            {
                string slnRelativePath = GetSafeRelativePath(_roslynSolution.FilePath);
                outputFiles[slnRelativePath] = await File.ReadAllTextAsync(_roslynSolution.FilePath);
            }

            foreach (var project in _roslynSolution.Projects)
            {
                // Skip in-memory projects that don't have a physical .csproj representation on disk.
                if (string.IsNullOrWhiteSpace(project.FilePath))
                    continue;

                string projectDir = Path.GetDirectoryName(project.FilePath)!;

                // 2. Prepare to save the project file (.csproj).
                string csprojRelativePath = GetSafeRelativePath(project.FilePath);
                outputFiles[csprojRelativePath] = await File.ReadAllTextAsync(project.FilePath);

                // 3. Prepare to save the mutated C# syntax trees from Roslyn's memory.
                foreach (var document in project.Documents)
                {
                    // Skip source-generated documents, in-memory files, or SDK-generated build artifacts inside obj/ and bin/
                    // that do not belong to the physical project source structure.
                    if (string.IsNullOrWhiteSpace(document.FilePath) || IsBuildArtifact(projectDir, document.FilePath))
                        continue;

                    // Calculate the relative path so the directory structure is perfectly mirrored in the output folder.
                    string relativePath = GetSafeRelativePath(document.FilePath);

                    // Retrieve the obfuscated source code text.
                    var text = await document.GetTextAsync();
                    outputFiles[relativePath] = text.ToString();
                }
            }

            // Flush everything to the disk concurrently.
            await _fileWriter.WriteAllFilesAsync(targetDirectoryPath, outputFiles);
        }

        /// <summary>
        /// Determines whether a document file path resides within the project's intermediate or output build directories (<c>obj</c> or <c>bin</c>).
        /// </summary>
        /// <param name="projectDir">The root directory of the owning project.</param>
        /// <param name="filePath">The full file path of the document to check.</param>
        /// <returns><see langword="true"/> if the file is located inside <c>obj</c> or <c>bin</c>; otherwise, <see langword="false"/>.</returns>
        private static bool IsBuildArtifact(string projectDir, string filePath)
        {
            string relativeToProject = Path.GetRelativePath(projectDir, filePath);
            string[] segments = relativeToProject.Split(
                [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                StringSplitOptions.RemoveEmptyEntries);

            return segments.Length > 0 &&
                   (segments[0].Equals("obj", StringComparison.OrdinalIgnoreCase) ||
                    segments[0].Equals("bin", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Calculates a path relative to the solution root and ensures it does not escape outside that directory.
        /// </summary>
        /// <param name="fullPath">The absolute path of the file to map.</param>
        /// <returns>A safe relative path inside the solution root directory.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the file resides outside the solution root directory.</exception>
        private string GetSafeRelativePath(string fullPath)
        {
            string relativePath = Path.GetRelativePath(_solutionRootPath, fullPath);

            bool escapesRoot = relativePath == ".."
                || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                || relativePath.StartsWith($"..{Path.AltDirectorySeparatorChar}", StringComparison.Ordinal)
                || Path.IsPathRooted(relativePath);

            if (escapesRoot)
                throw new InvalidOperationException($"File '{fullPath}' is located outside the solution root directory.");

            return relativePath;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ObfuscationService"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _workspace?.Dispose();
                    _workspace = null;

                    _roslynSolution = null;
                }

                _disposed = true;
            }
        }
    }
}
