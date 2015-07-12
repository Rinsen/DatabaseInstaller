using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class Table
    {
        public string Name { get; private set; }
        public List<Column> Columns { get; private set; }
        public NamedPrimaryKey NamedPrimaryKeys { get; private set; }
        public NamedUnique NamedUniques { get; private set; }

        public string CreateScript { get { return GetUpScript(); } }

        public Table(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }
            Columns = new List<Column>();
            NamedPrimaryKeys = new NamedPrimaryKey();
            NamedUniques = new NamedUnique();
            Name = name;
        }

        public Table InsertColumn(Column column)
        {
            Columns.Add(column);
            return this;
        }

        public ColumnBuilder AddColumn(string name, IDbType columnType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (Columns.Any(col => col.Name == name))
            {
                throw new ArgumentException(string.Format("A column with the name {0} already exist in table {1}", name, Name));
            }

            var columnBuilder = new ColumnBuilder(this, name, columnType);

            Columns.Add(columnBuilder.Column);

            return columnBuilder;
        }

        public string GetUpScript()
        {
            if (!Columns.Any())
                throw new InvalidOperationException("No colums found in table definition");

            var sb = new StringBuilder("CREATE TABLE ");
            sb.Append(Name);
            sb.AppendLine();
            sb.AppendLine("(");
            var lastColumn = Columns.Last();
            foreach (var column in Columns)
            {
                sb.AppendFormat("{0} {1}{2}", column.Name, column.ColumnType.GetSqlServerDatabaseTypeString() ,GetConstraintString(column));
                
                if (!column.Equals(lastColumn) || NamedPrimaryKeys.Any() || NamedUniques.Any())
                {
                    sb.Append(",");
                }
                sb.AppendLine();
            }

            if (NamedUniques.Any())
            {
                foreach (var namedUnique in NamedUniques)
                {
                    sb.AppendFormat("CONSTRAINT {0} UNIQUE ({1})", namedUnique.Key, FormatColumnNames(namedUnique.Value));
                    sb.AppendLine();
                }
            }

            if (NamedPrimaryKeys.Any())
            {
                foreach (var namedPrimaryKey in NamedPrimaryKeys)
                {
                    sb.AppendFormat("CONSTRAINT {0} PRIMARY KEY ({1})", namedPrimaryKey.Key, FormatColumnNames(namedPrimaryKey.Value));
                    sb.AppendLine();
                }
            }

            sb.Append(")");
            return sb.ToString();
        }

        private object FormatColumnNames(List<string> names)
        {
            var sb = new StringBuilder(names.First());
            for (int i = 1; i < names.Count; i++)
            {
                sb.AppendFormat(",{0}", names[i]);
            }
            return sb.ToString();
        }

        private string GetConstraintString(Column column)
        {
            var sb = new StringBuilder();

            if (column.NotNull && column.AutoIncrement == null)
            {
                sb.Append(" NOT NULL");
            }
            if (column.Unique)
            {
                sb.Append(" UNIQUE");
            }
            if (column.AutoIncrement != null)
            {
                sb.AppendFormat(" IDENTITY({0},{1})", column.AutoIncrement.StartValue, column.AutoIncrement.Increment);
            }
            if (column.PrimaryKey)
            {
                sb.Append(" PRIMARY KEY");
            }
            if (column.Check != null)
            {
                throw new NotImplementedException("Check is not implemented");
            }
            if (column.DefaultValue != null)
            {
                throw new NotImplementedException("Default value is not implemented");
            }
            if (column.ForeignKey != null)
            {
                sb.AppendFormat(" FOREIGN KEY REFERENCES {0}({1})", column.ForeignKey.TableName, column.ForeignKey.ColumnName);
            }

            return sb.ToString();
        }

        public string GetPrimaryKeyConstraintStandardName()
        {
            return string.Format("PK_{0}", Name);
        }
    }
}
