using Rinsen.DatabaseInstaller.SqlTypes;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller
{
    public class ColumnBuilder
    {
        public Column Column { get; }

        private readonly Table _table;

        public ColumnBuilder(Table table)
        {
            _table = table;
        }

        public ColumnBuilder(Table table, string name, IDbType columnType)
        {
            _table = table;
            Column = new Column(name, columnType);
        }

        public ColumnBuilder ForeignKey<T>(Expression<Func<T, object>> propertyExpression)
        {
            var tableName = typeof(T).Name + "s";
            var columnName = propertyExpression.GetMemberName();
            ForeignKey(tableName, columnName);
            return this;
        }

        public ColumnBuilder NotNull()
        {
            Column.Null = false;
            return this;
        }

        public ColumnBuilder Null()
        {
            Column.Null = true;
            return this;
        }

        public ColumnBuilder Unique()
        {
            if (Column.PrimaryKey)
            {
                throw new InvalidOperationException("A unique constraint can not be combined with primary key");
            }

            Column.Unique = true;
            return this;
        }

        public ColumnBuilder Clustered()
        {
            if (_table.PrimaryKeyClustered)
            {
                throw new InvalidOperationException("A clustered column can only be added if the primary key is not clustered");
            }

            Column.Clustered = true;
            return this;
        }

        public ColumnBuilder Unique(string name)
        {
            if (Column.PrimaryKey || _table.NamedPrimaryKeys.Any(m => m.Key == Column.Name))
            {
                throw new InvalidOperationException("A unique constraint can not be combined with a primary key on the same column");
            }

            _table.NamedUniques.Add(name, Column.Name);
            return this;
        }

        public ColumnBuilder PrimaryKey()
        {
            if (Column.Unique || _table.NamedUniques.Any(m => m.Key == Column.Name))
            {
                throw new InvalidOperationException("A primary key can not be combined with a unique constraint on the same column");
            }

            if (_table.Columns.Any(c => c.PrimaryKey && c.Name != Column.Name))
            {
                var constraintName = _table.GetPrimaryKeyConstraintStandardName();

                _table.NamedPrimaryKeys.Add(constraintName, Column.Name);

                AddAnyExistingPrimaryKeysToNamedPrimaryKeys(constraintName);
            }
            else
            {
                Column.PrimaryKey = true;
            }
            Column.Null = false;
            return this;
        }

        private void AddAnyExistingPrimaryKeysToNamedPrimaryKeys(string constraintName)
        {
            foreach (var column in _table.Columns.Where(c => c.PrimaryKey))
            {
                column.PrimaryKey = false;
                _table.NamedPrimaryKeys.Add(constraintName, column.Name);
            }
        }

        public ColumnBuilder PrimaryKey(string name)
        {
            if (_table.NamedPrimaryKeys.Count > 0 &&
                !_table.NamedPrimaryKeys.Keys.Contains(name))
            {
                throw new ArgumentException("Ony one named primary key can exist");
            }

            _table.NamedPrimaryKeys.Add(name, Column.Name);

            AddAnyExistingPrimaryKeysToNamedPrimaryKeys(name);

            Column.Null = false;
            return this;
        }

        public ColumnBuilder ForeignKey(string tableName)
        {
            return ForeignKey(tableName, Column.Name);
        }

        public ColumnBuilder ForeignKey(string tableName, string columnName)
        {
            Column.ForeignKey = new ForeignKey(tableName, columnName);
            return this;
        }

        public ColumnBuilder Check()
        {
            Column.Check = new Check();
            return this;
        }

        public ColumnBuilder DefaultValue()
        {
            Column.DefaultValue = new DefaultValue();
            return this;
        }

        public ColumnBuilder AutoIncrement(int startValue = 1, int increment = 1, bool primaryKey = true)
        {
            if (primaryKey)
            {
                PrimaryKey();
            }
            Column.AutoIncrement = new AutoIncrement(startValue, increment);
            return this;
        }
    }
}