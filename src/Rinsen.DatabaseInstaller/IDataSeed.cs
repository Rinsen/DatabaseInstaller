using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public interface IDataSeed
    {
        Task SeedData();
    }
}
