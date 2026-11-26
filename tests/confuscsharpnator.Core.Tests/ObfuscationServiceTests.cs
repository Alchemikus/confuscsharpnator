using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Moq;

namespace confuscsharpnator.Core.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="ObfuscationService"/> class, focusing on state management,
    /// validation guard clauses, step execution ordering, and proper resource disposal.
    /// </summary>
    public class ObfuscationServiceTests : IDisposable
    {
        private readonly Mock<IFileWriter> _fileWriterMock;
        private readonly Mock<IObfuscatorStep> _dummyStepMock;
        private readonly List<IObfuscatorStep> _injectedSteps;
        private readonly List<string> _tempDirectories = [];

        static ObfuscationServiceTests()
        {
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }
        }

        public ObfuscationServiceTests()
        {
            _fileWriterMock = new Mock<IFileWriter>();
            _dummyStepMock = CreateStepMock("Rename", executionOrder: 40);
            _injectedSteps = [_dummyStepMock.Object];
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task InitializeAsync_WithNullOrEmptyPath_ThrowsArgumentException(string? invalidPath)
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.InitializeAsync(invalidPath!));
            Assert.Contains("Invalid solution file path", exception.Message);
        }

        [Fact]
        public async Task InitializeAsync_WhenFileDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);
            string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "missing.sln");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.InitializeAsync(nonExistentPath));
        }

        [Fact]
        public void Obfuscate_WhenNotInitialized_ThrowsInvalidOperationException()
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);
            var selectedSteps = new List<string> { "Rename" };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => service.Obfuscate(selectedSteps));
            Assert.Contains("Service is not initialized", exception.Message);
        }

        [Fact]
        public async Task SaveAsync_WhenNotInitialized_ThrowsInvalidOperationException()
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);
            string targetPath = Path.Combine(Path.GetTempPath(), "output");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.SaveAsync(targetPath));
            Assert.Contains("No solution to save", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SaveAsync_WithNullOrEmptyTargetDirectory_ThrowsArgumentException(string? invalidTargetDir)
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);
            string slnPath = await CreateTempSolutionAsync("DummySolution.sln", TestContext.Current.CancellationToken);
            await service.InitializeAsync(slnPath);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.SaveAsync(invalidTargetDir!));
            Assert.Contains("Target directory path cannot be empty", exception.Message);
        }

        [Fact]
        public async Task Obfuscate_ExecutesOnlySelectedSteps_CaseInsensitive()
        {
            // Arrange
            var selectedStep = CreateStepMock("StringEncryption", executionOrder: 10);
            var unselectedStep = CreateStepMock("AntiDebug", executionOrder: 0);

            using var service = new ObfuscationService(_fileWriterMock.Object, [selectedStep.Object, unselectedStep.Object]);
            string slnPath = await CreateTempSolutionAsync("DummySolution.sln", TestContext.Current.CancellationToken);
            await service.InitializeAsync(slnPath);

            // Act
            service.Obfuscate(["stringencryption"]);

            // Assert
            selectedStep.Verify(s => s.Process(It.IsAny<Solution>(), It.IsAny<ObfuscationContext>()), Times.Once);
            unselectedStep.Verify(s => s.Process(It.IsAny<Solution>(), It.IsAny<ObfuscationContext>()), Times.Never);
        }

        [Fact]
        public async Task Obfuscate_RespectsExecutionOrderAndNameTieBreaker()
        {
            // Arrange
            var executionLog = new List<string>();

            var stepLast = CreateStepMock("ControlFlow", executionOrder: 30, onProcess: () => executionLog.Add("ControlFlow"));
            var stepTieB = CreateStepMock("MathObfuscation", executionOrder: 20, onProcess: () => executionLog.Add("MathObfuscation"));
            var stepTieA = CreateStepMock("callsHiding", executionOrder: 20, onProcess: () => executionLog.Add("callsHiding"));
            var stepFirst = CreateStepMock("AntiDebug", executionOrder: 0, onProcess: () => executionLog.Add("AntiDebug"));

            // Celowo wstrzykujemy kroki w wymieszanej kolejności
            var steps = new[] { stepLast.Object, stepTieB.Object, stepTieA.Object, stepFirst.Object };
            using var service = new ObfuscationService(_fileWriterMock.Object, steps);

            string slnPath = await CreateTempSolutionAsync("DummySolution.sln", TestContext.Current.CancellationToken);
            await service.InitializeAsync(slnPath);

            // Act
            service.Obfuscate(["ControlFlow", "MathObfuscation", "callsHiding", "AntiDebug"]);

            // Assert
            Assert.Equal(["AntiDebug", "callsHiding", "MathObfuscation", "ControlFlow"], executionLog);
        }

        [Fact]
        public void Dispose_WhenCalledMultipleTimes_DoesNotThrowException()
        {
            // Arrange
            var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);

            // Act
            service.Dispose();
            var exception = Record.Exception(() => service.Dispose());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task ObfuscationPipeline_HappyPath_ExecutesStepsAndSavesFiles()
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);
            string slnPath = await CreateTempSolutionAsync("DummySolution.sln", TestContext.Current.CancellationToken);
            string outputDir = Path.Combine(Path.GetDirectoryName(slnPath)!, "Output");

            Dictionary<string, string>? savedFiles = null;
            _fileWriterMock
                .Setup(fw => fw.WriteAllFilesAsync(outputDir, It.IsAny<Dictionary<string, string>>()))
                .Callback<string, Dictionary<string, string>>((_, files) => savedFiles = files)
                .Returns(Task.CompletedTask);

            // Act
            await service.InitializeAsync(slnPath);
            service.Obfuscate(["Rename"]);
            await service.SaveAsync(outputDir);

            // Assert
            _dummyStepMock.Verify(s => s.Process(
                It.IsAny<Solution>(),
                It.IsAny<ObfuscationContext>()),
                Times.Once);

            _fileWriterMock.Verify(fw => fw.WriteAllFilesAsync(
                outputDir,
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);

            Assert.NotNull(savedFiles);
            Assert.True(savedFiles.ContainsKey("DummySolution.sln"));
        }

        // --- Helper methods ---

        private static Mock<IObfuscatorStep> CreateStepMock(string name, uint executionOrder, Action? onProcess = null)
        {
            var mock = new Mock<IObfuscatorStep>();
            mock.Setup(s => s.Name).Returns(name);
            mock.Setup(s => s.ExecutionOrder).Returns(executionOrder);
            mock.Setup(s => s.Process(It.IsAny<Solution>(), It.IsAny<ObfuscationContext>()))
                .Returns((Solution sol, ObfuscationContext _) =>
                {
                    onProcess?.Invoke();
                    return sol;
                });

            return mock;
        }

        private async Task<string> CreateTempSolutionAsync(string fileName, CancellationToken cancellationToken)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            _tempDirectories.Add(tempDir);

            string slnPath = Path.Combine(tempDir, fileName);
            await File.WriteAllTextAsync(
                slnPath,
                "Microsoft Visual Studio Solution File, Format Version 12.00",
                cancellationToken);

            return slnPath;
        }

        public void Dispose()
        {
            foreach (var dir in _tempDirectories)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, recursive: true);
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}