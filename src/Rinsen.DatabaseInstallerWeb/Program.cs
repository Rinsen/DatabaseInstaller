using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rinsen.DatabaseInstallerWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddEnvironmentVariables();

                    if (env.IsDevelopment())
                    {
                        // This reads the configuration keys from the secret store.
                        // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                        config.AddUserSecrets<Startup>();
                    }
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConsole();
                })
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
