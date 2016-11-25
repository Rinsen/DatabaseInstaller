namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class ForeignKey
    {
        public ForeignKey(string tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }

        public string TableName { get; private set; }

        public string ColumnName { get; private set; }
    }
}