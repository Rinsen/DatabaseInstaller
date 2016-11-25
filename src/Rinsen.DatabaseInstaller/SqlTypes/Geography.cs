namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class Geography : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "geography";
        }
    }
}
