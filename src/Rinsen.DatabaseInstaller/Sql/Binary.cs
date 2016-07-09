namespace Rinsen.DatabaseInstaller.Sql
{
    public class Binary : IDbType
    {
        readonly int _length;

        public Binary(int length)
        {
            _length = length;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            return string.Format("binary({0})", _length);
        }
    }
}
