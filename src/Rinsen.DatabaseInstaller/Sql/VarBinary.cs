namespace Rinsen.DatabaseInstaller.Sql
{
    public class VarBinary : IDbType
    {
        private readonly int _length;

        public VarBinary(int length = 0)
        {
            _length = length;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            if (_length == 0)
            {
                return "varbinary(max)";
            }
            return string.Format("varbinary({0})", _length);
        }
    }
}
