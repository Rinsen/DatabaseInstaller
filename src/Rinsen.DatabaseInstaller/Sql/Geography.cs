namespace Rinsen.DatabaseInstaller.Sql
{
    public class Geography : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "geography";
        }
    }
}
