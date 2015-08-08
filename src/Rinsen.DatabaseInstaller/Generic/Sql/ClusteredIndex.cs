using Rinsen.DatabaseInstaller.Sql;
using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Generic.Sql
{
    public class ClusteredIndex<T> : ClusteredIndex
    {
        public ClusteredIndex(string name, string tableName)
            : base (name, tableName)
        { }

        public void AddColumn(Expression<Func<T, object>> propertyExpression)
        {
            var name = propertyExpression.GetMemberName();

            AddColumn(name);
        }

    }
}
