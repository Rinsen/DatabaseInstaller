namespace Rinsen.DatabaseInstaller.Sql
{
    public class DateTimeOffset : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "datetimeoffset";
        }
    }
}
