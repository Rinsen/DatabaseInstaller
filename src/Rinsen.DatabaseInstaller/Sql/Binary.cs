namespace Rinsen.DatabaseInstaller.Sql
{
    public class Binary : IDbType
    {
        private readonly int _length;

        public Binary(int length = 0)
        {
            _length = length;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            if (_length == 0)
            {
                return "binary(max)";
            }
            return string.Format("binary({0})", _length);
        }
    }
}
