using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class TableAlteration : IDbChange
    {
        public string TableName { get; set; }
        public List<ColumnToAdd> ColumnsToAdd { get; set; }
        public List<string> ColumnsToDelete { get; set; }

        public TableAlteration(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Table name is mandatory");
            }
            TableName = name;
            ColumnsToAdd = new List<ColumnToAdd>();
            ColumnsToDelete = new List<string>();
        }

        public ColumnToAddBuilder AddColumn(string name, IDbType type)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (ColumnsToAdd.Any(col => col.Name == name))
            {
                throw new ArgumentException(string.Format("A column with the name {0} already exist in table alteration {1}", name, TableName));
            }

            var columnBuilder = new ColumnToAddBuilder(this, name, type);

            ColumnsToAdd.Add(columnBuilder.ColumnToAdd);

            return columnBuilder;
        }

        public void DeleteColumn(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (ColumnsToDelete.Any(col => col == name))
            {
                throw new ArgumentException(string.Format("A column with the name {0} already exist in table alteration {1}", name, TableName));
            }

            ColumnsToDelete.Add(name);
        }

        public List<string> GetUpScript()
        {
            var scripts = new List<string>();

            if (!ColumnsToAdd.Any() && !ColumnsToDelete.Any())
                throw new InvalidOperationException("No colums found in alter table definition");

            

            if (ColumnsToDelete.Any())
            {
                var sb = new StringBuilder(string.Format("ALTER TABLE {0}", TableName));
                sb.AppendLine();

                foreach (var columnToDelete in ColumnsToDelete)
                {
                    if (ColumnsToDelete.IndexOf(columnToDelete) == ColumnsToDelete.Count - 1)
                    {
                        sb.AppendLine(string.Format("DROP COLUMN {0}", columnToDelete));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("DROP COLUMN {0},", columnToDelete));
                    }
                }
                scripts.Add(sb.ToString());
            }

            if (ColumnsToAdd.Any())
            {
                var sb = new StringBuilder(string.Format("ALTER TABLE {0} ADD", TableName));
                sb.AppendLine();

                foreach (var columnToAdd in ColumnsToAdd)
                {
                    if (ColumnsToAdd.IndexOf(columnToAdd) == ColumnsToAdd.Count - 1)
                    {
                        sb.AppendLine(string.Format("{0} {1}{2}", columnToAdd.Name, columnToAdd.Type.GetSqlServerDatabaseTypeString(), GetConstraintString(columnToAdd)));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("{0} {1}{2},", columnToAdd.Name, columnToAdd.Type.GetSqlServerDatabaseTypeString(), GetConstraintString(columnToAdd)));
                    }
                }

                scripts.Add(sb.ToString());
            }

            return scripts;
        }

        private string GetConstraintString(ColumnToAdd column)
        {
            var sb = new StringBuilder();

            if (column.NotNull)
            {
                sb.Append(" NOT NULL");
            }
            if (column.Unique)
            {
                sb.Append(" UNIQUE");
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

        public List<string> GetDownScript()
        {
            throw new NotImplementedException();
        }
    }
}
