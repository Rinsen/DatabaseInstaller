using System;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public class AddValue : IDbChange
    {
        public string TableName { get; }

        public string ColumnName { get; set; } = string.Empty;


        public AddValue(string tableName)
        {
            ArgumentNullException.ThrowIfNull(tableName);

            TableName = tableName;
        }   

        public IReadOnlyList<string> GetUpScript(InstallerOptions installerOptions)
        {
            if (string.IsNullOrEmpty(ColumnName))
            {
                throw new NotSupportedException("Empty column name is not supported.");
            }

            return [$"UPDATE [{installerOptions.DatabaseName}].[{installerOptions.Schema}].[{TableName}]{Environment.NewLine}SET {ColumnName} = NEWID(){Environment.NewLine}WHERE {ColumnName} is NULL"];
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
