using System;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class VarBinary : IDbType
    {
        private readonly int _length;
        private readonly bool _max;

        public VarBinary()
        {
            _max = true;
        }

        public VarBinary(int length)
        {
            if (length < 1)
            {
                throw new ArgumentException("Value has to be a positive number", nameof(length));
            }
            if (length > 8000)
            {
                throw new ArgumentException("Max supported value is 8000", nameof(length));
            }

            _length = length;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            if (_max)
            {
                return "varbinary(max)";
            }

            return string.Format("varbinary({0})", _length);
        }
    }
}
