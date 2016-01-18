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

        public static ColumnBuilder AddColumn<T>(this Table<T> table, Expression<Func<T, object>> propertyExpression, int? length = null, int? precision = null)
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

                return table.AddColumn(propertyExpression, new DateTime());
            }
            if (propertyType == typeof(System.DateTimeOffset))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type");

                return table.AddColumn(propertyExpression, new DateTimeOffset());
            }
            if (propertyType == typeof(System.DateTimeOffset?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type");

                return table.AddColumn(propertyExpression, new DateTimeOffset());
            }
            if (propertyType == typeof(int))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type");

                return table.AddColumn(propertyExpression, new Int());
            }
            if (propertyType == typeof(int?))
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
            if (propertyType == typeof(byte[]))
            {
                if (length == null)
                {
                    return table.AddColumn(propertyExpression, new Binary());
                }
                return table.AddColumn(propertyExpression, new Binary((int)length));
            }
            if (propertyType == typeof(decimal))
            {
                if (length == null && precision == null)
                {
                    return table.AddColumn(propertyExpression, new Decimal(5, 2));
                }
                if (length == null || precision == null)
                {
                    throw new ArgumentException("Length and precision is mandatory for this type if one is provided");
                }
                return table.AddColumn(propertyExpression, new Decimal((int)length, (int)precision));
            }


            throw new ArgumentException("Type is not supported");
        }
    }
}
