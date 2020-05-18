using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Rinsen.DatabaseInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace Database
{
    class Program
    {
        public static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        [Argument(0, Description = "Command to execute")]
        [Required]
        public string Command { get; }

        [Option("--ConnectionString", Description = "Db connection string")]
        public string ConnectionString { get; }

        [Option("--Path")]
        public string Path{ get; }

        [Option("--AssemblyName")]
        public string AssemblyName { get; }

        private async Task OnExecuteAsync()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDatabaseInstaller(ConnectionString);
                    services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
                    services.AddHostedService<DatabaseInstallerHostedService>();
                    services.AddTransient<DatabaseInstallationHandler>();
                    services.AddSingleton(new DatabaseInstallerOptions
                    {
                        AssemblyName = AssemblyName,
                        Command = Command,
                        Path = Path
                    });
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                    });
                });

            var host = hostBuilder.Build();

            await host.RunAsync();
        }
    }
}
