using System;

namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class Binary : IDbType
    {
        readonly int _length;

        public Binary(int length)
        {
            if (length < 1 || length > 8000)
            {
                throw new ArgumentException("Positive length or less then 8000 is mandatory for this type", nameof(length));
            }
            _length = length;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            return string.Format("binary({0})", _length);
        }
    }
}
