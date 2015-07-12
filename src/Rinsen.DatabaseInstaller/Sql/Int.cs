using System;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class Int : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "int";
        }
    }
}
