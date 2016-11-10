using Rinsen.DatabaseInstaller.Sql;
using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Sql.Generic
{
    public static class TableAlterationExtensionMethods
    {

        public static ColumnToAddBuilder AddColumn<T>(this TableAlteration<T> table, Expression<Func<T, object>> propertyExpression, int? length = null, int? precision = null)
        {
            var propertyType = propertyExpression.GetMemberType();

            if (propertyType == typeof(bool))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new Bit()).NotNull();
            }

            if (propertyType == typeof(bool?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new Bit());
            }

            if (propertyType == typeof(System.DateTime))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new DateTime()).NotNull();
            }

            if (propertyType == typeof(System.DateTime?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new DateTime());
            }

            if (propertyType == typeof(System.DateTimeOffset))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new DateTimeOffset()).NotNull();
            }

            if (propertyType == typeof(System.DateTimeOffset?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new DateTimeOffset());
            }

            if (propertyType == typeof(int))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new Int()).NotNull();
            }

            if (propertyType == typeof(int?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new Int());
            }

            if (propertyType == typeof(string))
            {
                if (length == null)
                {
                    return table.AddColumn(propertyExpression, new NVarChar());
                }
                return table.AddColumn(propertyExpression, new NVarChar((int)length)).NotNull();
            }

            if (propertyType == typeof(byte[]))
            {
                if (length == null)
                {
                    return table.AddColumn(propertyExpression, new VarBinary()).NotNull();
                }

                return table.AddColumn(propertyExpression, new VarBinary((int)length)).NotNull();
            }

            if (propertyType == typeof(byte?[]))
            {
                if (length == null)
                {
                    return table.AddColumn(propertyExpression, new VarBinary());
                }

                return table.AddColumn(propertyExpression, new VarBinary((int)length));
            }

            if (propertyType == typeof(decimal))
            {
                if (length == null && precision == null)
                {
                    return table.AddColumn(propertyExpression, new Decimal(5, 2)).NotNull();
                }
                if (length == null || precision == null)
                {
                    throw new ArgumentException("Length and precision is mandatory for this type if one is provided");
                }
                return table.AddColumn(propertyExpression, new Decimal((int)length, (int)precision)).NotNull();
            }

            if (propertyType == typeof(System.Guid))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new Guid()).NotNull();
            }

            if (propertyType == typeof(System.Guid?))
            {
                if (length != null)
                    throw new ArgumentException("Length is not supported for this type", nameof(length));

                return table.AddColumn(propertyExpression, new Guid());
            }

            throw new ArgumentException("Type is not supported");
        }
    }
}
