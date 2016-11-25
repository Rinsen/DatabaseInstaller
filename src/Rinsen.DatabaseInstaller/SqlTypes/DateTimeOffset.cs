namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class DateTimeOffset : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "datetimeoffset";
        }
    }
}
