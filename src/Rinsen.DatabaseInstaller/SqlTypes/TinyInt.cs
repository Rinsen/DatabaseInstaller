namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class TinyInt : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "tinyint";
        }
    }
}
