using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    /// <summary>
    /// This should only be used when debugging!!!
    /// </summary>
    public class DatabaseInstallerMiddleware
    {
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
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
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (!_installed)
                {
                    await _installer.RunAsync(_options.Value.DatabaseVersions);
                    _installed = true;

                    await _next.Invoke(context);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
