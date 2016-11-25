namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class Bit : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "bit";
        }
    }
}
