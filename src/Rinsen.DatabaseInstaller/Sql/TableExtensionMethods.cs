using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Rinsen.DatabaseInstaller.Sql
{
    public static class TableExtensionMethods
    {
        public static ColumnBuilder AddAutoIncrementColumn(this Table table, string name)
        {
            return table.AddColumn(name, new Int()).AutoIncrement();
        }

        public static ColumnBuilder AddAutoIncrementColumn<T>(this Table<T> table, Expression<Func<T, object>> property)
        {
            return table.AddColumn(property, new Int()).AutoIncrement();
        }

        public static ColumnBuilder AddBitColumn(this Table table, string name)
        {
            return table.AddColumn(name, new Bit());
        }

        public static ColumnBuilder AddDateTimeOffsetColumn(this Table table, string name)
        {
            return table.AddColumn(name, new DateTimeOffset());
        }

        public static ColumnBuilder AddIntColumn(this Table table, string name)
        {
            return table.AddColumn(name, new Int());
        }

        public static ColumnBuilder AddNCharColumn(this Table table, string name, int length)
        {
            return table.AddColumn(name, new NChar(length));
        }

        public static ColumnBuilder AddNVarCharColumn(this Table table, string name, int length)
        {
            return table.AddColumn(name, new NVarChar(length));
        }

        public static ColumnBuilder AddBinaryColumn(this Table table, string name, int length)
        {
            return table.AddColumn(name, new Binary(length));
        }

        public static ColumnBuilder AddGeographyColumn(this Table table, string name)
        {
            return table.AddColumn(name, new Geography());
        }

        public static ColumnBuilder AddDecimalColumn(this Table table, string name, int precision, int scale)
        {
            return table.AddColumn(name, new Decimal(precision, scale));
        }
    }
}
