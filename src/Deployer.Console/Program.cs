using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;

namespace Deployer.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            ConfigureLogging();

            // v4: Log core architecture, AMD Shell, and Lumia support info
            ArchitectureDetector.LogArchitectureInfo();

            var consoleSession = DeployerConsoleSessionFactory.Create();
            await consoleSession.Run(args);
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Logs\\log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}", LogEventLevel.Information)
                .MinimumLevel.Verbose()
                .CreateLogger();
        }
    }
}
