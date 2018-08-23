using System;

namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class DateTime : IDbType
    {
        private bool _defaultSize;
        private int _scale;

        public DateTime()
        {
            _defaultSize = true;
        }

        public DateTime(int scale)
        {
            if (scale > 7 || scale < 0)
            {
                throw new NotSupportedException("DateTime supports a scale between 0 and 7");
            }

            _scale = scale;
        }

        public string GetSqlServerDatabaseTypeString()
        {
            if (_defaultSize)
            {
                return "datetime2";
            }

            return $"datetime2({_scale})";
        }
    }
}
