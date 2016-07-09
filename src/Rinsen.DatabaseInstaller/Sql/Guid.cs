namespace Rinsen.DatabaseInstaller.Sql
{
    public class Guid : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "uniqueidentifier";
        }
    }
}
