using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.DatabaseInstaller
{
    public class AddValue : IDbChange
    {
        public string TableName { get; }

        public string ColumnName { get; set; }


        public AddValue(string tableName)
        {
            TableName = tableName;
        }

        public List<string> GetUpScript()
        {
            return new List<string> { $"UPDATE {TableName}\r\nSET {ColumnName} = NEWID()\r\nWHERE {ColumnName} is NULL" };
        }

        public void GuidColumn(string columnName)
        {
            ColumnName = columnName;
        }

        public List<string> GetDownScript()
        {
            throw new NotImplementedException();
        }
    }
}
