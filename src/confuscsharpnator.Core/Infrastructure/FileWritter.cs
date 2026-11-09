using confuscsharpnator.Core.Abstractions;

namespace confuscsharpnator.Core.Infrastructure
{
    /// <summary>
    /// Provides physical file system operations for the obfuscator, 
    /// executing file writes concurrently for maximum I/O performance.
    /// </summary>
    public class FileWriter : IFileWriter
    {
        /// <summary>
        /// Asynchronously and concurrently writes a collection of files to the specified directory.
        /// </summary>
        /// <param name="targetDirectoryPath">The absolute path to the root output directory.</param>
        /// <param name="files">A dictionary where the key is the relative file path and the value is the text content.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteAllFilesAsync(string targetDirectoryPath, Dictionary<string, string> files)
        {
            var writeTasks = files.Select(async kvp =>
            {
                string relativePath = kvp.Key;
                string content = kvp.Value;

                string fullPath = Path.Combine(targetDirectoryPath, relativePath);
                string directory = Path.GetDirectoryName(fullPath)!;

                Directory.CreateDirectory(directory);

                await File.WriteAllTextAsync(fullPath, content);
            });

            await Task.WhenAll(writeTasks);
        }
    }
}