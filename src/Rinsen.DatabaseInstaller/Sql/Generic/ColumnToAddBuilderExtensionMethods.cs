using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Sql.Generic
{
    public static class ColumnToAddBuilderExtensionMethods
    {
        public static ColumnToAddBuilder ForeignKey<T>(this ColumnToAddBuilder columnBuilder, Expression<Func<T, object>> propertyExpression)
        {
            var tableName = typeof(T).Name + "s";
            var columnName = propertyExpression.GetMemberName();
            columnBuilder.ForeignKey(tableName, columnName);
            return columnBuilder;
        }
    }
}
