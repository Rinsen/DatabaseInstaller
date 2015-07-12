namespace Rinsen.DatabaseInstaller.Sql
{
    public class DateTime : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "datetime2";
        }
    }
}
