namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class BigInt : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "bigint";
        }
    }
}
