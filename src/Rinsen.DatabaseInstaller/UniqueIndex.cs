using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Rinsen.DatabaseInstaller
{
    public class UniqueIndex<T> : UniqueIndex
    {
        public UniqueIndex(string name, string tableName)
            : base(name, tableName)
        { }

        public void AddColumn(Expression<Func<T, object>> propertyExpression)
        {
            var name = propertyExpression.GetMemberName();

            AddColumn(name);
        }
    }

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
