﻿using Rinsen.DatabaseInstaller.SqlTypes;
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
            PrimaryKeyNonClustered = true;

            return this;
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
                    return AddColumn(propertyExpression, new SqlTypes.DateTime((int)length)).NotNull();

                return AddColumn(propertyExpression, new SqlTypes.DateTime()).NotNull();
            }

            if (propertyType == typeof(System.DateTime?))
            {
                if (length != null)
                    return AddColumn(propertyExpression, new SqlTypes.DateTime((int)length));

                return AddColumn(propertyExpression, new SqlTypes.DateTime());
            }

            if (propertyType == typeof(System.DateTimeOffset))
            {
                if (length != null)
                    return AddColumn(propertyExpression, new SqlTypes.DateTimeOffset((int)length)).NotNull();

                return AddColumn(propertyExpression, new SqlTypes.DateTimeOffset()).NotNull();
            }

            if (propertyType == typeof(System.DateTimeOffset?))
            {
                if (length != null)
                    return AddColumn(propertyExpression, new SqlTypes.DateTimeOffset((int)length));

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

            if (propertyType == typeof(byte))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new TinyInt()).NotNull();
            }

            if (propertyType == typeof(byte?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new TinyInt());
            }

            if (propertyType == typeof(short))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SmallInt()).NotNull();
            }

            if (propertyType == typeof(short?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SmallInt());
            }

            if (propertyType == typeof(long))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new BigInt()).NotNull();
            }

            if (propertyType == typeof(long?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new BigInt());
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
                    return AddColumn(propertyExpression, new SqlTypes.Decimal()).NotNull();
                }
                if (length == null || precision == null)
                {
                    throw new ArgumentException("Length and precision is mandatory for this type if one is provided");
                }
                return AddColumn(propertyExpression, new SqlTypes.Decimal((int)length, (int)precision)).NotNull();
            }

            if (propertyType == typeof(decimal?))
            {
                if (length == null && precision == null)
                {
                    return AddColumn(propertyExpression, new SqlTypes.Decimal());
                }
                if (length == null || precision == null)
                {
                    throw new ArgumentException("Length and precision is mandatory for this type if one is provided");
                }
                return AddColumn(propertyExpression, new SqlTypes.Decimal((int)length, (int)precision));
            }

            if (propertyType == typeof(double))
            {
                if (length == null)
                {
                    return AddColumn(propertyExpression, new Float()).NotNull();
                }

                return AddColumn(propertyExpression, new Float((int)length)).NotNull();
            }

            if (propertyType == typeof(double?))
            {
                if (length == null)
                {
                    return AddColumn(propertyExpression, new Float());
                }

                return AddColumn(propertyExpression, new Float((int)length));
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

            if (propertyType.IsEnum)
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return AddColumn(propertyExpression, new SqlTypes.TinyInt()).NotNull();
            }

            throw new ArgumentException($"Property Type '{propertyType}' is not supported");
        }
    }

    public class Table : IDbChange
    {
        protected bool _alternation = false;

        public string Name { get; }
        public List<Column> Columns { get; } = new List<Column>();
        public List<string> ColumnsToDelete { get; } = new List<string>();
        public NamedPrimaryKey NamedPrimaryKeys { get; } = new NamedPrimaryKey();
        public NamedUnique NamedUniques { get; } = new NamedUnique();
        public bool PrimaryKeyNonClustered { get; protected set; }

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
            if (_alternation)
            {
                return GetAlterationScripts();
            }

            if (!Columns.Any())
                throw new InvalidOperationException("No colums found in table definition");

            var sb = new StringBuilder("CREATE TABLE ");
            sb.AppendFormat("[{0}]", Name);
            sb.AppendLine();
            sb.AppendLine("(");
            var lastColumn = Columns.Last();
            foreach (var column in Columns)
            {
                sb.AppendFormat("[{0}] {1}{2}", column.Name, column.Type.GetSqlServerDatabaseTypeString(), GetConstraintString(column));

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
                    if (PrimaryKeyNonClustered)
                    {
                        sb.AppendFormat("CONSTRAINT {0} PRIMARY KEY NONCLUSTERED ({1})", namedPrimaryKey.Key, FormatColumnNames(namedPrimaryKey.Value));
                    }
                    else
                    {
                        sb.AppendFormat("CONSTRAINT {0} PRIMARY KEY ({1})", namedPrimaryKey.Key, FormatColumnNames(namedPrimaryKey.Value));
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

            if (!Columns.Any() && !Columns.Any())
                throw new InvalidOperationException("No colums found in alter table definition");

            if (ColumnsToDelete.Any())
            {
                var sb = new StringBuilder(string.Format("ALTER TABLE [{0}]", Name));
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

            if (Columns.Any())
            {
                var sb = new StringBuilder(string.Format("ALTER TABLE {0} ADD", Name));
                sb.AppendLine();

                foreach (var columnToAdd in Columns)
                {
                    if (Columns.IndexOf(columnToAdd) == Columns.Count - 1)
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
            if (!column.NotNull && column.AutoIncrement == null)
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

                if (PrimaryKeyNonClustered)
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
            PrimaryKeyNonClustered = true;

            return this;
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

        public ColumnBuilder AddTinyIntColumn(string name)
        {
            return AddColumn(name, new TinyInt());
        }

        public ColumnBuilder AddSmallIntColumn(string name)
        {
            return AddColumn(name, new SmallInt());
        }

        public ColumnBuilder AddBigIntColumn(string name)
        {
            return AddColumn(name, new BigInt());
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

        public ColumnBuilder AddFloatColumn(string name, int n)
        {
            return AddColumn(name, new Float(n));
        }
    }
}
