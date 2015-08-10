using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Sql.Generic
{
    public static class TableExtensionMethods
    {

        public static ColumnBuilder AddAutoIncrementColumn<T>(this Table<T> table, Expression<Func<T, object>> property)
        {
            return table.AddColumn(property, new Int()).AutoIncrement();
        }

        public static ColumnBuilder AddColumn<T>(this Table<T> table, Expression<Func<T, object>> propertyExpression, int? length = null)
        {
            var propertyType = propertyExpression.GetMemberType();

            if (propertyType == typeof(bool))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type");

                return table.AddColumn(propertyExpression, new Bit());
            }
            if (propertyType == typeof(System.DateTime))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type");

                return table.AddColumn(propertyExpression, new DatabaseInstaller.Sql.DateTime());
            }
            if (propertyType == typeof(System.DateTimeOffset))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type");

                return table.AddColumn(propertyExpression, new DatabaseInstaller.Sql.DateTimeOffset());
            }
            if (propertyType == typeof(int))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type");

                return table.AddColumn(propertyExpression, new Int());
            }
            if (propertyType == typeof(string))
            {
                if (length == null)
                {
                    return table.AddColumn(propertyExpression, new NVarChar());
                }
                return table.AddColumn(propertyExpression, new NVarChar((int)length));
            }

            throw new ArgumentException("Type is not supported");
        }
    }
}
