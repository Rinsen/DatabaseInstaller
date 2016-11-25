using Rinsen.DatabaseInstaller.SqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Rinsen.DatabaseInstaller
{
    public class Table<T> : Table
    {
        public Table(string name)
            : base(name)
        { }

        public ColumnBuilder AddColumn(Expression<Func<T, object>> propertyExpression, IDbType columnType)
        {
            var name = propertyExpression.GetMemberName();

            return AddColumn(name, columnType);
        }

        public ColumnBuilder AddAutoIncrementColumn(Expression<Func<T, object>> property, int startValue = 1, int increment = 1, bool primaryKey = true)
        {
            return AddColumn(property, new Int()).AutoIncrement(startValue: startValue, increment: increment, primaryKey: primaryKey);
        }

        public ColumnBuilder AddColumn(Expression<Func<T, object>> propertyExpression, int? length = null, int? precision = null)
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

    public class Table : IDbChange
    {
        public string Name { get; private set; }
        public List<Column> Columns { get; private set; }
        public NamedPrimaryKey NamedPrimaryKeys { get; private set; }
        public NamedUnique NamedUniques { get; private set; }

        public Table(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Table name is mandatory");
            }
            Columns = new List<Column>();
            NamedPrimaryKeys = new NamedPrimaryKey();
            NamedUniques = new NamedUnique();
            Name = name;
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

        public List<string> GetUpScript()
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
                sb.AppendFormat("{0} {1}{2}", column.Name, column.Type.GetSqlServerDatabaseTypeString() ,GetConstraintString(column));
                
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

            return new List<string> { sb.ToString() };
        }

        public List<string> GetDownScript()
        {
            throw new NotImplementedException();
        }

        string FormatColumnNames(List<string> names)
        {
            var sb = new StringBuilder(names.First());
            for (int i = 1; i < names.Count; i++)
            {
                sb.AppendFormat(",{0}", names[i]);
            }
            return sb.ToString();
        }

        string GetConstraintString(Column column)
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

        public ColumnBuilder AddAutoIncrementColumn(string name)
        {
            return AddColumn(name, new Int()).AutoIncrement();
        }

        public ColumnBuilder AddBitColumn(string name)
        {
            return AddColumn(name, new Bit());
        }

        public ColumnBuilder AddDateTimeOffsetColumn(string name)
        {
            return AddColumn(name, new SqlTypes.DateTimeOffset());
        }

        public ColumnBuilder AddIntColumn(string name)
        {
            return AddColumn(name, new Int());
        }

        public ColumnBuilder AddNCharColumn(string name, int length)
        {
            return AddColumn(name, new NChar(length));
        }

        public ColumnBuilder AddNVarCharColumn(string name, int length)
        {
            return AddColumn(name, new NVarChar(length));
        }

        public ColumnBuilder AddBinaryColumn(string name, int length)
        {
            return AddColumn(name, new Binary(length));
        }

        public ColumnBuilder AddGeographyColumn(string name)
        {
            return AddColumn(name, new Geography());
        }

        public ColumnBuilder AddDecimalColumn(string name, int precision, int scale)
        {
            return AddColumn(name, new SqlTypes.Decimal(precision, scale));
        }
    }
}
