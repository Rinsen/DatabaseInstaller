namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class SmallInt : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "smallint";
        }
    }
}
