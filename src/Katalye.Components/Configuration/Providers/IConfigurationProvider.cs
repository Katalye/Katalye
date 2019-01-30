namespace Katalye.Components.Configuration.Providers
{
    public interface IConfigurationProvider
    {
        int Priority { get; }

        (bool Success, string Value ) TryGet(string path, string defaultValue);
    }
}