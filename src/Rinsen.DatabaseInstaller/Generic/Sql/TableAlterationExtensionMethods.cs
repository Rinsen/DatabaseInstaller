using Rinsen.DatabaseInstaller.Sql;
using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Generic.Sql
{
    public static class TableAlterationExtensionMethods
    {

        public static ColumnToAddBuilder AddColumn<T>(this TableAlteration<T> table, Expression<Func<T, object>> propertyExpression, int? length = null)
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
