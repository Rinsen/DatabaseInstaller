using System;

namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class Float : IDbType
    {
        private readonly int _n;

        /// <summary>
        /// n value = 53
        /// </summary>
        public Float()
        {
            _n = 53;
        }

        /// <summary>
        /// SQL Server treats n as one of two possible values. If 1<=n<=24, n is treated as 24 (4 bytes). If 25<=n<=53, n is treated as 53 (8 bytes). 
        /// </summary>
        /// <param name="n">represents precision</param>
        public Float(int n)
        {
            if (n < 1)
            {
                throw new ArgumentException("Value has to be a positive number", nameof(n));
            }
            if (n > 53)
            {
                throw new ArgumentException("Max supported value is 53", nameof(n));
            }

            if (n <= 24)
            {
                _n = 24;
            }

            _n = 53;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            return $"float({_n})";
        }
    }
}
