using System;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class Decimal : IDbType
    {
        private readonly int _precision;
        private readonly int _scale;

        /// <summary>
        /// For more details see https://msdn.microsoft.com/en-us/library/ms187746.aspx
        /// </summary>
        /// <param name="precision">The maximum total number of decimal digits that will be stored, both to the left and to the right of the decimal point. </param>
        /// <param name="scale">The number of decimal digits that will be stored to the right of the decimal point.</param>
        public Decimal(int precision, int scale)
        {
            _precision = precision;
            _scale = scale;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            return string.Format("decimal({0},{1})", _precision, _scale);
        }
    }
}
