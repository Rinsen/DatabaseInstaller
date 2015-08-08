using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class UniqueIndex : Index
    {
        public UniqueIndex(string name, string tableName)
            : base(name, tableName)
        { }

        public override List<string> GetUpScript()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("CREATE UNIQUE INDEX {0} ", Name);
            AddTableInformation(sb);

            return new List<string> { sb.ToString() };
        }

        public override List<string> GetDownScript()
        {
            throw new NotImplementedException();
        }
    }
}
