namespace confuscsharpnator.Shared
{
    public interface IObfuscationFeatureInfo
    {
        string Name { get; }
        string DisplayName { get; }
        bool IsEnabledByDefault => true;
    }
}
