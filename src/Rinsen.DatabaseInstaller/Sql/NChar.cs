using System;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class NChar : IDbType
    {
        private int _length;
        private bool max = false;

        /// <summary>
        /// Create a nvarchar(max)
        /// </summary>
        public NChar()
        {
            max = true;
        }

        /// <summary>
        /// Create a nvarchar(length)
        /// </summary>
        public NChar(int length)
        {
            if (length < 1 || length > 4000)
            {
                throw new ArgumentException("nchar length can be a value from 1 through 4,000");
            }

            _length = length;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            if (max)
            {
                return "nchar(max)";
            }
            return string.Format("nchar({0})", _length);
        }
    }
}
