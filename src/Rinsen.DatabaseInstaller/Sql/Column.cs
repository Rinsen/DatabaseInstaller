namespace Rinsen.DatabaseInstaller.Sql
{
    public class Column
    {
        public Column()
        {
            NotNull = false;
            Unique = false;
            PrimaryKey = false;
            ForeignKey = null;
            Check = null;
            DefaultValue = null;
        }

        public string Name { get; set; }

        public IDbType ColumnType { get; set; }

        public bool NotNull { get; set; }

        public bool Unique { get; set; }

        public bool PrimaryKey { get; set; }

        public ForeignKey ForeignKey { get; set; }

        public Check Check { get; set; }

        public DefaultValue DefaultValue { get; set; }

        public AutoIncrement AutoIncrement { get; set; }
    }
}