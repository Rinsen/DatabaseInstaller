using System;

namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class Int : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "int";
        }
    }
}
