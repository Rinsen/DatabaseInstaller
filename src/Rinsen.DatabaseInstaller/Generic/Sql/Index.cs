using System;
using Rinsen.DatabaseInstaller.Sql;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Generic.Sql
{
    public class Index<T> : Index
    {
        public Index(string name, string tableName)
            : base(name, tableName)
        { }

        public void AddColumn(Expression<Func<T, object>> propertyExpression)
        {
            var name = propertyExpression.GetMemberName();

            AddColumn(name);
        }
    }
}
