namespace confuscsharpnator.Core.Abstractions
{
    public interface IObfuscationService : IDisposable
    {
        Task InitializeAsync(string csprojFilePath);
        void Obfuscate(IEnumerable<string> selectedStepNames);
        Task SaveAsync(string targetDirectoryPath);
    }
}