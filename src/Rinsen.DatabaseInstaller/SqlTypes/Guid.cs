namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class Guid : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "uniqueidentifier";
        }
    }
}
