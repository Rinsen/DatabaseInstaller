namespace Rinsen.DatabaseInstaller.Sql
{
    public class Bit : IDbType
    {
        public string GetSqlServerDatabaseTypeString()
        {
            return "bit";
        }
    }
}
