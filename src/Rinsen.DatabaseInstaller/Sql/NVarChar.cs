using System;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class NVarChar : IDbType
    {
        private int _length;
        private bool max = false;

        /// <summary>
        /// Create a nvarchar(max)
        /// </summary>
        public NVarChar()
        {
            max = true;
        }

        /// <summary>
        /// Create a nvarchar(length)
        /// </summary>
        public NVarChar(int length)
        {
            if (length < 1 || length > 4000)
            {
                throw new ArgumentException("nvarchar length can be a value from 1 through 4,000");
            }

            _length = length;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            if (max)
            {
                return "nvarchar(max)";
            }
            return string.Format("nvarchar({0})", _length);
        }
    }
}
