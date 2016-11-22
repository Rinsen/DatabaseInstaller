using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public class VersionOptions : IOptions<VersionOptions>
    {
        public List<DatabaseVersion> DatabaseVersions { get; } = new List<DatabaseVersion>();

        public VersionOptions Value { get { return this; } }
    }
}
