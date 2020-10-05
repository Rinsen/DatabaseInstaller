using System;
using System.Collections.Generic;

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

        public IReadOnlyList<string> GetUpScript(InstallerOptions installerOptions)
        {
            return new List<string> { $"UPDATE [{installerOptions.DatabaseName}].[{installerOptions.Schema}].[{TableName}]{Environment.NewLine}SET {ColumnName} = NEWID(){Environment.NewLine}WHERE {ColumnName} is NULL" };
        }

        public void GuidColumn(string columnName)
        {
            ColumnName = columnName;
        }

        public IReadOnlyList<string> GetDownScript(InstallerOptions installerOptions)
        {
            throw new NotImplementedException();
        }
    }
}
