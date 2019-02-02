using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Katalye.Components.Configuration.Providers;
using NLog;

namespace Katalye.Components.Configuration
{
    public interface IConfigurationRenderer
    {
        IEnumerable<RenderedSetting<string>> RenderSettings();
        RenderedSetting<string> RenderSetting(string path);
        RenderedSetting<object> RenderSetting(string path, Type type);
    }

    public class ConfigurationRenderer : IConfigurationRenderer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<HashSet<string>> Paths
            = new Lazy<HashSet<string>>(CreatePaths);

        private readonly CreateValueParser _createValueParser;
        private readonly IList<IConfigurationProvider> _providers;

        public ConfigurationRenderer(IList<IConfigurationProvider> providers, CreateValueParser createValueParser)
        {
            _createValueParser = createValueParser;
            _providers = providers
                         .OrderBy(x => x.Priority)
                         .ToList();
        }

        public IEnumerable<RenderedSetting<string>> RenderSettings()
        {
            foreach (var path in Paths.Value)
            {
                yield return RenderSetting(path);
            }
        }

        public RenderedSetting<string> RenderSetting(string path)
        {
            foreach (var provider in _providers)
            {
                var (success, value) = provider.TryGet(path);
                if (success)
                {
                    return new RenderedSetting<string>
                    {
                        Key = path.ToLower(),
                        Value = value,
                        Provider = provider.Name
                    };
                }
            }

            throw new Exception("No configuration providers succeeded.");
        }

        public RenderedSetting<object> RenderSetting(string path, Type type)
        {
            var setting = RenderSetting(path);
            var parsedObj = ParseResult(type, setting.Value, path);

            return new RenderedSetting<object>
            {
                Key = setting.Key,
                Value = parsedObj,
                Provider = setting.Provider
            };
        }

        private static HashSet<string> CreatePaths()
        {
            var configType = typeof(IKatalyeConfiguration);
            var properties = configType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var paths = properties.Select(x => x.GetCustomAttribute<ConfigurationMemberAttribute>())
                                  .Where(x => x != null)
                                  .Select(x => x.Path);

            var result = new HashSet<string>(paths);
            return result;
        }

        private object ParseResult(Type type, string value, string path)
        {
            object result = null;
            try
            {
                var parser = _createValueParser.Invoke(type);
                if (parser != null)
                {
                    result = parser.Parse(value);
                }
                else
                {
                    Logger.Error($"For configuration path [{path}] no parser for type {type.FullName} exists, returning a default value.");
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, $"For configuration path [{path}] failed to parse value [{value}] to type [{type.FullName}], "
                               + "a default value will be used.");
            }

            return result ?? GetDefaultValue(type);
        }

        private object GetDefaultValue(Type t)
        {
            // https://stackoverflow.com/a/2490274/2001966
            return t.IsValueType
                ? Activator.CreateInstance(t)
                : null;
        }
    }

    public class RenderedSetting<T>
    {
        public string Key { get; set; }

        public T Value { get; set; }

        public string Provider { get; set; }
    }
}