using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public static class DatabaseInstallerAppBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseInstaller(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<DatabaseInstallerMiddleware>();
        }

        public static IApplicationBuilder UseDatabaseInstaller(this IApplicationBuilder app, Action<VersionOptions> options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var versionOptions = new VersionOptions();
            options.Invoke(versionOptions);

            return app.UseMiddleware<DatabaseInstallerMiddleware>(versionOptions);
        }
    }
}
