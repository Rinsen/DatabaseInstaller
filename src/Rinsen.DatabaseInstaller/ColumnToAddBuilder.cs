using Rinsen.DatabaseInstaller.SqlTypes;
using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller
{
    public class ColumnToAddBuilder
    {
        private readonly TableAlteration _tableAlteration;
        public ColumnToAdd ColumnToAdd { get; set; }

        public ColumnToAddBuilder(TableAlteration tableAlteration, string name, IDbType type)
        {
            _tableAlteration = tableAlteration;
            ColumnToAdd = new ColumnToAdd { Name = name, Type = type };
        }

        public ColumnToAddBuilder ForeignKey<T>(Expression<Func<T, object>> propertyExpression)
        {
            var tableName = typeof(T).Name + "s";
            var columnName = propertyExpression.GetMemberName();
            ForeignKey(tableName, columnName);
            return this;
        }

        public ColumnToAddBuilder NotNull()
        {
            ColumnToAdd.NotNull = true;
            return this;
        }

        public ColumnToAddBuilder Null()
        {
            ColumnToAdd.NotNull = false;
            return this;
        }

        public ColumnToAddBuilder Unique()
        {
            ColumnToAdd.Unique = true;
            return this;
        }

        public ColumnToAddBuilder ForeignKey(string tableName)
        {
            return ForeignKey(tableName, ColumnToAdd.Name);
        }

        public ColumnToAddBuilder ForeignKey(string tableName, string columnName)
        {
            ColumnToAdd.ForeignKey = new ForeignKey(tableName, columnName);
            return this;
        }

        public ColumnToAddBuilder Check()
        {
            ColumnToAdd.Check = new Check();
            return this;
        }

        public ColumnToAddBuilder DefaultValue()
        {
            ColumnToAdd.DefaultValue = new DefaultValue();
            return this;
        }
    }
}