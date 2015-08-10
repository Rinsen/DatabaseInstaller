using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Sql.Generic
{
    public static class ColumnBuilderExtensionMethods
    {
        public static ColumnBuilder ForeignKey<T>(this ColumnBuilder columnBuilder, Expression<Func<T, object>> propertyExpression)
        {
            var tableName = typeof(T).Name + "s";
            var columnName = propertyExpression.GetMemberName();
            columnBuilder.ForeignKey(tableName, columnName);
            return columnBuilder;
        }
    }
}
