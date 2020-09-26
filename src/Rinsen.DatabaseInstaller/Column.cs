using Rinsen.DatabaseInstaller.SqlTypes;

namespace Rinsen.DatabaseInstaller
{
    public class Column
    {
        internal Column(string name, IDbType dbType)
        {
            Name = name;
            DbType = dbType;
        }

        public string Name { get; }

        public IDbType DbType { get; }

        public bool Null { get; internal set; } = false;

        public bool Unique { get; internal set; } = false;

        public bool Clustered { get; internal set; } = false;

        public bool PrimaryKey { get; internal set; } = false;

        public ForeignKey ForeignKey { get; internal set; } = null;

        public Check Check { get; internal set; } = null;

        public DefaultValue DefaultValue { get; internal set; } = null;

        public AutoIncrement AutoIncrement { get; internal set; } = null;
    }
}
