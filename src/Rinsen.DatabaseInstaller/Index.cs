using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller
{
    public class Index<T> : Index
    {
        public Index(string name, string tableName)
            : base(name, tableName)
        { }

        public void AddColumn(Expression<Func<T, object>> propertyExpression)
        {
            var name = propertyExpression.GetMemberName();

            AddColumn(name);
        }

        public new Index<T> Clustered()
        {
            IsClustered = true;

            return this;
        }

        public new Index<T> Unique()
        {
            IsUnique = true;

            return this;
        }
    }

    public class Index : IDbChange
    {
        public string IndexName { get; private set; }

        public string TableName { get; private set; }

        public List<string> Columns { get; private set; } = new List<string>();

        public bool IsClustered { get; protected set; }

        public bool IsUnique { get; protected set; }

        public Index(string indexName, string tableName)
        {
            IndexName = indexName;
            TableName = tableName;
        }

        public Index AddColumn(string name)
        {
            if (Columns.Contains(name))
            {
                throw new ArgumentException($"The column {name} is already added to index {IndexName} on table {TableName}");
            }

            Columns.Add(name);

            return this;
        }

        public Index Clustered()
        {
            IsClustered = true;

            return this;
        }

        public Index Unique()
        {
            IsUnique = true;

            return this;
        }

        public IReadOnlyList<string> GetUpScript(InstallerOptions installerOptions)
        {
            var sb = new StringBuilder();

            if (IsClustered && IsUnique)
            {
                sb.AppendFormat("CREATE UNIQUE CLUSTERED INDEX {0} ", IndexName);
            }
            else if (IsClustered) 
            {
                sb.AppendFormat("CREATE CLUSTERED INDEX {0} ", IndexName);
            }
            else if (IsUnique)
            {
                sb.AppendFormat("CREATE UNIQUE INDEX {0} ", IndexName);
            }
            else
            {
                sb.AppendFormat("CREATE INDEX {0} ", IndexName);
            }
            
            AddTableInformation(sb, installerOptions);

            return new List<string> { sb.ToString() };
        }

        private void AddTableInformation(StringBuilder sb, InstallerOptions installerOptions)
        {
            if (!Columns.Any())
            {
                throw new InvalidOperationException($"No columns is added to index {IndexName} for table {TableName}");
            }

            sb.AppendLine();
            sb.AppendFormat("ON [{0}].[{1}].[{2}] ", installerOptions.DatabaseName, installerOptions.Schema, TableName);
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

        public IReadOnlyList<string> GetDownScript(InstallerOptions installerOptions)
        {
            throw new NotImplementedException();
        }
    }
}
