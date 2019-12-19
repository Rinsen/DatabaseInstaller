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
        protected Table(string name, bool alternation)
            : this(name)
        {
            _alternation = alternation;
        }

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

        public new Table<T> SetPrimaryKeyNonClustered()
        {
            PrimaryKeyClustered = false;

            return this;
        }

        public ColumnBuilder AddColumn(Expression<Func<T, object>> propertyExpression, int? length = null, int? precision = null)
        {
            var propertyType = propertyExpression.GetMemberType();
            var columnTypeResult = GetColumnTypeResult(propertyType, length, precision);

            var columnBuilder = AddColumn(propertyExpression, columnTypeResult.DbType);

            if (columnTypeResult.Null)
            {
                columnBuilder.Null();
            }

            return columnBuilder;
        }

        protected ColumnTypeResult GetColumnTypeResult(Type propertyType, int? length, int? precision)
        {
            if (propertyType == typeof(bool))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new Bit());
            }

            if (propertyType == typeof(bool?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new Bit(), true);
            }

            if (propertyType == typeof(System.DateTime))
            {
                if (length != null)
                    return new ColumnTypeResult(new SqlTypes.DateTime((int)length));

                return new ColumnTypeResult(new SqlTypes.DateTime());
            }

            if (propertyType == typeof(System.DateTime?))
            {
                if (length != null)
                    return new ColumnTypeResult(new SqlTypes.DateTime((int)length));

                return new ColumnTypeResult(new SqlTypes.DateTime(), true);
            }

            if (propertyType == typeof(System.DateTimeOffset))
            {
                if (length != null)
                    return new ColumnTypeResult(new SqlTypes.DateTimeOffset((int)length));

                return new ColumnTypeResult(new SqlTypes.DateTimeOffset());
            }

            if (propertyType == typeof(System.DateTimeOffset?))
            {
                if (length != null)
                    return new ColumnTypeResult(new SqlTypes.DateTimeOffset((int)length));

                return new ColumnTypeResult(new SqlTypes.DateTimeOffset(), true);
            }

            if (propertyType == typeof(int))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new Int());
            }

            if (propertyType == typeof(int?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new Int(), true);
            }

            if (propertyType == typeof(byte))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new TinyInt());
            }

            if (propertyType == typeof(byte?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new TinyInt(), true);
            }

            if (propertyType == typeof(short))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new SmallInt());
            }

            if (propertyType == typeof(short?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new SmallInt(), true);
            }

            if (propertyType == typeof(long))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new BigInt());
            }

            if (propertyType == typeof(long?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new BigInt(), true);
            }

            if (propertyType == typeof(string))
            {
                if (length == null)
                {
                    return new ColumnTypeResult(new NVarChar(1));
                }
                if (length == int.MaxValue)
                {
                    return new ColumnTypeResult(new NVarChar());
                }
                return new ColumnTypeResult(new NVarChar((int)length));
            }

            if (propertyType == typeof(byte[]))
            {
                if (length == null)
                {
                    return new ColumnTypeResult(new VarBinary());
                }

                return new ColumnTypeResult(new VarBinary((int)length));
            }

            if (propertyType == typeof(byte?[]))
            {
                if (length == null)
                {
                    return new ColumnTypeResult(new VarBinary(), true);
                }

                return new ColumnTypeResult(new VarBinary((int)length), true);
            }

            if (propertyType == typeof(decimal))
            {
                if (length == null && precision == null)
                {
                    return new ColumnTypeResult(new SqlTypes.Decimal());
                }
                if (length == null || precision == null)
                {
                    throw new ArgumentException("Length and precision is mandatory for this type if one is provided");
                }
                return new ColumnTypeResult(new SqlTypes.Decimal((int)length, (int)precision));
            }

            if (propertyType == typeof(decimal?))
            {
                if (length == null && precision == null)
                {
                    return new ColumnTypeResult(new SqlTypes.Decimal(), true);
                }
                if (length == null || precision == null)
                {
                    throw new ArgumentException("Length and precision is mandatory for this type if one is provided");
                }
                return new ColumnTypeResult(new SqlTypes.Decimal((int)length, (int)precision), true);
            }

            if (propertyType == typeof(double))
            {
                if (length == null)
                {
                    return new ColumnTypeResult(new Float());
                }

                return new ColumnTypeResult(new Float((int)length));
            }

            if (propertyType == typeof(double?))
            {
                if (length == null)
                {
                    return new ColumnTypeResult(new Float(), true);
                }

                return new ColumnTypeResult(new Float((int)length), true);
            }

            if (propertyType == typeof(System.Guid))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new SqlTypes.Guid());
            }

            if (propertyType == typeof(System.Guid?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new SqlTypes.Guid(), true);
            }

            if (propertyType.IsEnum)
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return new ColumnTypeResult(new TinyInt());
            }

            throw new ArgumentException($"Property Type '{propertyType}' is not supported");
        }

        protected class ColumnTypeResult
        {
            public ColumnTypeResult(IDbType dbType, bool nullable = false)
            {
                DbType = dbType;
                Null = nullable;
            }

            public IDbType DbType { get; }

            public bool Null { get;  }
        }
    }

    public class Table : IDbChange
    {
        protected bool _alternation = false;

        public string Name { get; }
        public List<Column> ColumnsToAdd { get; } = new List<Column>();
        public List<Column> ColumnsToAlter { get; } = new List<Column>();
        public List<string> ColumnsToDrop { get; } = new List<string>();
        public NamedPrimaryKey NamedPrimaryKeys { get; } = new NamedPrimaryKey();
        public NamedUnique NamedUniques { get; } = new NamedUnique();
        public bool PrimaryKeyClustered { get; protected set; } = true;

        protected Table(string name, bool alternation)
            :this(name)
        {
            _alternation = alternation;
        }

        public Table(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Table name is mandatory");
            }

            Name = name;
        }

        public ColumnBuilder AddColumn(string name, IDbType columnType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (ColumnsToAdd.Any(col => col.Name == name))
            {
                throw new ArgumentException(string.Format("A column with the name {0} already exist in table {1}", name, Name));
            }

            var columnBuilder = new ColumnBuilder(this, name, columnType);

            ColumnsToAdd.Add(columnBuilder.Column);

            return columnBuilder;
        }

        public List<string> GetUpScript()
        {
            if (_alternation)
            {
                return GetAlterationScripts();
            }

            if (!ColumnsToAdd.Any())
                throw new InvalidOperationException("No colums found in table definition");

            var sb = new StringBuilder("CREATE TABLE ");
            sb.AppendFormat("[{0}]", Name);
            sb.AppendLine();
            sb.AppendLine("(");
            var lastColumn = ColumnsToAdd.Last();
            foreach (var column in ColumnsToAdd)
            {
                sb.AppendFormat("[{0}] {1}{2}", column.Name, column.DbType.GetSqlServerDatabaseTypeString(), GetConstraintString(column));

                if (!column.Equals(lastColumn) || NamedPrimaryKeys.Any() || NamedUniques.Any())
                {
                    sb.Append(",");
                }
                sb.AppendLine();
            }

            AddNamedUniques(sb);
            AddNamedPrimaryKeys(sb);

            sb.Append(")");

            return new List<string> { sb.ToString() };
        }

        private void AddNamedPrimaryKeys(StringBuilder sb)
        {
            if (NamedPrimaryKeys.Any())
            {
                var lastNamedPrimaryKeys = NamedPrimaryKeys.Last();

                foreach (var namedPrimaryKey in NamedPrimaryKeys)
                {
                    if (PrimaryKeyClustered)
                    {
                        sb.AppendFormat("CONSTRAINT {0} PRIMARY KEY ({1})", namedPrimaryKey.Key, FormatColumnNames(namedPrimaryKey.Value));
                    }
                    else
                    {
                        sb.AppendFormat("CONSTRAINT {0} PRIMARY KEY NONCLUSTERED ({1})", namedPrimaryKey.Key, FormatColumnNames(namedPrimaryKey.Value));
                    }

                    if (!lastNamedPrimaryKeys.Equals(namedPrimaryKey))
                    {
                        sb.Append(",");
                    }

                    sb.AppendLine();
                }
            }
        }

        private void AddNamedUniques(StringBuilder sb)
        {
            if (NamedUniques.Any())
            {
                sb.AppendLine($"ALTER TABLE {Name} ADD");

                var lastNamedUnique = NamedUniques.Last();

                foreach (var namedUnique in NamedUniques)
                {
                    sb.AppendFormat("CONSTRAINT {0} UNIQUE ({1})", namedUnique.Key, FormatColumnNames(namedUnique.Value));

                    if (!lastNamedUnique.Equals(namedUnique) || NamedPrimaryKeys.Any())
                    {
                        sb.Append(",");
                    }

                    sb.AppendLine();
                }
            }
        }

        private List<string> GetAlterationScripts()
        {
            var scripts = new List<string>();

            if (!ColumnsToAdd.Any() && !ColumnsToAdd.Any() && !ColumnsToAlter.Any())
                throw new InvalidOperationException("No colums found in alter table definition");

            if (ColumnsToDrop.Any())
            {
                var sb = new StringBuilder($"ALTER TABLE [{Name}]");
                sb.AppendLine();

                foreach (var columnToDrop in ColumnsToDrop)
                {
                    if (ColumnsToDrop.IndexOf(columnToDrop) == ColumnsToDrop.Count - 1)
                    {
                        sb.AppendLine($"DROP COLUMN {columnToDrop}");
                    }
                    else
                    {
                        sb.AppendLine($"DROP COLUMN {columnToDrop},");
                    }
                }

                scripts.Add(sb.ToString());
            }

            if (ColumnsToAdd.Any())
            {
                var sb = new StringBuilder($"ALTER TABLE {Name} ADD");
                sb.AppendLine();

                foreach (var columnToAdd in ColumnsToAdd)
                {
                    if (ColumnsToAdd.IndexOf(columnToAdd) == ColumnsToAdd.Count - 1)
                    {
                        sb.AppendLine($"{columnToAdd.Name} {columnToAdd.DbType.GetSqlServerDatabaseTypeString()}{GetConstraintString(columnToAdd)}");
                    }
                    else
                    {
                        sb.AppendLine($"{columnToAdd.Name} {columnToAdd.DbType.GetSqlServerDatabaseTypeString()}{GetConstraintString(columnToAdd)},");
                    }
                }

                AddNamedUniques(sb);
                AddNamedPrimaryKeys(sb);

                scripts.Add(sb.ToString());
            }

            if (ColumnsToAlter.Any())
            {
                var sb = new StringBuilder($"ALTER TABLE {Name} ALTER");
                sb.AppendLine();
                
                foreach (var columnToAdd in ColumnsToAlter)
                {
                    if (ColumnsToAlter.IndexOf(columnToAdd) == ColumnsToAlter.Count - 1)
                    {
                        sb.AppendLine($"COLUMN [{columnToAdd.Name}] {columnToAdd.DbType.GetSqlServerDatabaseTypeString()}{GetConstraintString(columnToAdd)}");
                    }
                    else
                    {
                        sb.AppendLine($"COLUMN [{columnToAdd.Name}] {columnToAdd.DbType.GetSqlServerDatabaseTypeString()}{GetConstraintString(columnToAdd)},");
                    }
                }

                AddNamedUniques(sb);
                AddNamedPrimaryKeys(sb);

                scripts.Add(sb.ToString());

            }

            return scripts;
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

            if (!column.Null && column.AutoIncrement == null)
            {
                sb.Append(" NOT NULL");
            }
            if (column.Null && column.AutoIncrement == null)
            {
                sb.Append(" NULL");
            }
            if (column.Unique)
            {
                sb.Append(" UNIQUE");
            }
            if (column.Clustered)
            {
                sb.Append(" CLUSTERED");
            }
            if (column.AutoIncrement != null)
            {
                sb.AppendFormat(" IDENTITY({0},{1})", column.AutoIncrement.StartValue, column.AutoIncrement.Increment);
            }
            if (column.PrimaryKey)
            {
                sb.Append(" PRIMARY KEY");

                if (!PrimaryKeyClustered)
                {
                    sb.Append(" NONCLUSTERED");
                }
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

        public Table SetPrimaryKeyNonClustered()
        {
            PrimaryKeyClustered = false;

            return this;
        }

        public ColumnBuilder AddAutoIncrementColumn(string name)
        {
            return AddColumn(name, new Int()).AutoIncrement();
        }

        public ColumnBuilder AddBigIntColumn(string name)
        {
            return AddColumn(name, new BigInt());
        }

        public ColumnBuilder AddBinaryColumn(string name, int length)
        {
            return AddColumn(name, new Binary(length));
        }

        public ColumnBuilder AddBitColumn(string name)
        {
            return AddColumn(name, new Bit());
        }

        public ColumnBuilder AddDateTimeColumn(string name)
        {
            return AddColumn(name, new SqlTypes.DateTime());
        }

        public ColumnBuilder AddDateTimeColumn(string name, int precision)
        {
            return AddColumn(name, new SqlTypes.DateTime(precision));
        }

        public ColumnBuilder AddDateTimeOffsetColumn(string name)
        {
            return AddColumn(name, new SqlTypes.DateTimeOffset());
        }

        public ColumnBuilder AddDateTimeOffsetColumn(string name, int precision)
        {
            return AddColumn(name, new SqlTypes.DateTimeOffset(precision));
        }

        public ColumnBuilder AddDecimalColumn(string name, int precision, int scale)
        {
            return AddColumn(name, new SqlTypes.Decimal(precision, scale));
        }

        public ColumnBuilder AddFloatColumn(string name, int n)
        {
            return AddColumn(name, new Float(n));
        }

        public ColumnBuilder AddGeographyColumn(string name)
        {
            return AddColumn(name, new Geography());
        }

        public ColumnBuilder AddGuidColumn(string name)
        {
            return AddColumn(name, new SqlTypes.Guid());
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

        public ColumnBuilder AddSmallIntColumn(string name)
        {
            return AddColumn(name, new SmallInt());
        }

        public ColumnBuilder AddTinyIntColumn(string name)
        {
            return AddColumn(name, new TinyInt());
        }

        public ColumnBuilder AddVarBinaryColumn(string name, int length)
        {
            return AddColumn(name, new VarBinary(length));
        }

    }
}
