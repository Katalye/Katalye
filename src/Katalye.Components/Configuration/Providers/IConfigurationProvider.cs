namespace Katalye.Components.Configuration.Providers
{
    public interface IConfigurationProvider
    {
        int Priority { get; }

        string Name { get; }

        (bool Success, string Value) TryGet(string path);
    }
}