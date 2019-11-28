using Rinsen.DatabaseInstaller.SqlTypes;

namespace Rinsen.DatabaseInstaller
{
    public class Column
    {
        public Column(string name, IDbType dbType)
        {
            Name = name;
            DbType = dbType;
        }

        public string Name { get; }

        public IDbType DbType { get; }

        public bool Null { get; set; } = false;

        public bool Unique { get; set; } = false;

        public bool Clustered { get; set; } = false;

        public bool PrimaryKey { get; set; } = false;

        public ForeignKey ForeignKey { get; set; } = null;

        public Check Check { get; set; } = null;

        public DefaultValue DefaultValue { get; set; } = null;

        public AutoIncrement AutoIncrement { get; set; } = null;
    }
}