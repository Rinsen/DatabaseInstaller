using System;

namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class DateTimeOffset : IDbType
    {
        private readonly bool _defaultSize;
        private readonly int _scale;

        public DateTimeOffset()
        {
            _defaultSize = true;
        }

        public DateTimeOffset(int scale)
        {
            if (scale > 7 || scale < 0)
            {
                throw new NotSupportedException("DateTimeOffset supports a scale between 0 and 7");
            }

            _scale = scale;
        }


        public string GetSqlServerDatabaseTypeString()
        {
            if (_defaultSize)
            {
                return "datetimeoffset";
            }

            return $"datetimeoffset({_scale})";
        }
    }
}
