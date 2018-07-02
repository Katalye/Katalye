using System.Threading.Tasks;

using JetBrains.Annotations;

using McMaster.Extensions.CommandLineUtils;

using NLog;

namespace Katalye.Exporter
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        [UsedImplicitly]
        private async Task OnExecuteAsync()
        {
        }
    }
}