using Rinsen.DatabaseInstaller.SqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Rinsen.DatabaseInstaller
{
    public class TableAlteration<T> : TableAlteration
    {
        public TableAlteration(string name)
            : base(name)
        { }

        public ColumnToAddBuilder AddColumn(Expression<Func<T, object>> propertyExpression, IDbType columnType)
        {
            return AddColumn(propertyExpression.GetMemberName(), columnType);
        }

        public ColumnToAddBuilder AddColumn(Expression<Func<T, object>> propertyExpression, int? length = null, int? precision = null)
        {
            var propertyType = propertyExpression.GetMemberType();

            if (propertyType == typeof(bool))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new Bit()).NotNull();
            }

            if (propertyType == typeof(bool?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new Bit());
            }

            if (propertyType == typeof(System.DateTime))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SqlTypes.DateTime()).NotNull();
            }

            if (propertyType == typeof(System.DateTime?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SqlTypes.DateTime());
            }

            if (propertyType == typeof(System.DateTimeOffset))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SqlTypes.DateTimeOffset()).NotNull();
            }

            if (propertyType == typeof(System.DateTimeOffset?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SqlTypes.DateTimeOffset());
            }

            if (propertyType == typeof(int))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new Int()).NotNull();
            }

            if (propertyType == typeof(int?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new Int());
            }

            if (propertyType == typeof(string))
            {
                if (length == null)
                {
                    return AddColumn(propertyExpression, new NVarChar());
                }
                return AddColumn(propertyExpression, new NVarChar((int)length)).NotNull();
            }

            if (propertyType == typeof(byte[]))
            {
                if (length == null)
                {
                    return AddColumn(propertyExpression, new VarBinary()).NotNull();
                }

                return AddColumn(propertyExpression, new VarBinary((int)length)).NotNull();
            }

            if (propertyType == typeof(byte?[]))
            {
                if (length == null)
                {
                    return AddColumn(propertyExpression, new VarBinary());
                }

                return AddColumn(propertyExpression, new VarBinary((int)length));
            }

            if (propertyType == typeof(decimal))
            {
                if (length == null && precision == null)
                {
                    return AddColumn(propertyExpression, new SqlTypes.Decimal(5, 2)).NotNull();
                }
                if (length == null || precision == null)
                {
                    throw new ArgumentException("Length and precision is mandatory for this type if one is provided");
                }
                return AddColumn(propertyExpression, new SqlTypes.Decimal((int)length, (int)precision)).NotNull();
            }

            if (propertyType == typeof(System.Guid))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SqlTypes.Guid()).NotNull();
            }

            if (propertyType == typeof(System.Guid?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SqlTypes.Guid());
            }

            throw new ArgumentException("Type is not supported");
        }
    }

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

        public ColumnToAddBuilder AddBitColumn(string name)
        {
            return AddColumn(name, new Bit());
        }

        public ColumnToAddBuilder AddDateTimeOffsetColumn(string name)
        {
            return AddColumn(name, new SqlTypes.DateTimeOffset());
        }

        public ColumnToAddBuilder AddIntColumn(string name)
        {
            return AddColumn(name, new Int());
        }

        public ColumnToAddBuilder AddNCharColumn(string name, int length)
        {
            return AddColumn(name, new NChar(length));
        }

        public ColumnToAddBuilder AddNVarCharColumn(string name, int length)
        {
            return AddColumn(name, new NVarChar(length));
        }

        public ColumnToAddBuilder AddBinaryColumn(string name, int length)
        {
            return AddColumn(name, new Binary(length));
        }

        public ColumnToAddBuilder AddGeographyColumn(string name)
        {
            return AddColumn(name, new Geography());
        }

        public ColumnToAddBuilder AddDecimalColumn(string name, int precision, int scale)
        {
            return AddColumn(name, new SqlTypes.Decimal(precision, scale));
        }

        public List<string> GetDownScript()
        {
            throw new NotImplementedException();
        }
    }
}
