namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class DateTime : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "datetime2";
        }
    }
}
