namespace confuscsharpnator.Core.Abstractions
{
    public interface IFileWriter
    {
        Task WriteAllFilesAsync(string targetDirectoryPath, Dictionary<string, string> files);
    }
}
