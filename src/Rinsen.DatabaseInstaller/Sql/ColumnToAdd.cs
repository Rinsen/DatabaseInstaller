namespace Rinsen.DatabaseInstaller.Sql
{
    public class ColumnToAdd
    {
        public ColumnToAdd()
        {
            NotNull = false;
            Unique = false;
            ForeignKey = null;
            DefaultValue = null;
            Check = null;
        }

        public string Name { get; set; }

        public IDbType Type { get; set; }

        public bool NotNull { get; set; }

        public bool Unique { get; set; }

        public ForeignKey ForeignKey { get; set; }

        public DefaultValue DefaultValue { get; set; }

        public Check Check { get; set; }
    }
}