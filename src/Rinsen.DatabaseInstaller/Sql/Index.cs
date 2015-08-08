using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class Index : IDbChange
    {
        public string Name { get; private set; }

        public string TableName { get; private set; }

        public List<string> Columns { get; private set; }

        public Index(string name, string tableName)
        {
            Name = name;
            TableName = tableName;
            Columns = new List<string>();
        }

        public void AddColumn(string name)
        {
            if (Columns.Contains(name))
            {
                throw new ArgumentException(string.Format("The column {0} is already added to index {1} on table {2}", name, Name, TableName));
            }

            Columns.Add(name);
        }

        public virtual List<string> GetUpScript()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("CREATE INDEX {0} ", Name);
            AddTableInformation(sb);

            return new List<string> { sb.ToString() };
        }

        protected void AddTableInformation(StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendFormat("ON {0}", TableName);
            sb.Append("(");

            var last = Columns.Last();
            foreach (var column in Columns)
            {
                sb.Append(column);
                if (column != last)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendLine(")");
        }

        public virtual List<string> GetDownScript()
        {
            throw new NotImplementedException();
        }
    }
}
