namespace Rinsen.DatabaseInstaller.Sql
{
    public static class TableAlterationExtensionMethods
    {

        public static ColumnToAddBuilder AddBitColumn(this TableAlteration table, string name)
        {
            return table.AddColumn(name, new Bit());
        }

        public static ColumnToAddBuilder AddDateTimeOffsetColumn(this TableAlteration table, string name)
        {
            return table.AddColumn(name, new DateTimeOffset());
        }

        public static ColumnToAddBuilder AddIntColumn(this TableAlteration table, string name)
        {
            return table.AddColumn(name, new Int());
        }

        public static ColumnToAddBuilder AddNCharColumn(this TableAlteration table, string name, int length)
        {
            return table.AddColumn(name, new NChar(length));
        }

        public static ColumnToAddBuilder AddNVarCharColumn(this TableAlteration table, string name, int length)
        {
            return table.AddColumn(name, new NVarChar(length));
        }

        public static ColumnToAddBuilder AddBinaryColumn(this TableAlteration table, string name, int length)
        {
            return table.AddColumn(name, new Binary(length));
        }

        public static ColumnToAddBuilder AddGeographyColumn(this TableAlteration table, string name)
        {
            return table.AddColumn(name, new Geography());
        }

        public static ColumnToAddBuilder AddDecimalColumn(this TableAlteration table, string name, int precision, int scale)
        {
            return table.AddColumn(name, new Decimal(precision, scale));
        }
    }
}
