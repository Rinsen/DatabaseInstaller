using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public class DatabaseInstallerMiddleware
    {
        private static readonly object _lock = new object();
        private static bool _installed = false;

        private readonly RequestDelegate _next;
        private readonly Installer _installer;
        private IOptions<VersionOptions> _options;

        public DatabaseInstallerMiddleware(RequestDelegate next, Installer installer, IOptions<VersionOptions> options)
        {
            _next = next;
            _installer = installer;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            lock (_lock)
            {
                if (!_installed)
                {
                    _installer.Run(_options.Value.DatabaseVersions);
                    _installed = true;
                }
            }

            await _next.Invoke(context);

        }
    }
}
