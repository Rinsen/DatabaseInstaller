using Rinsen.DatabaseInstaller.Sql;
using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Generic.Sql
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
}
