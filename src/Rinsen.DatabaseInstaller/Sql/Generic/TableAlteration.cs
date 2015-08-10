using Rinsen.DatabaseInstaller.Sql;
using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Sql.Generic
{
    public class TableAlteration<T> : TableAlteration
    {
        public TableAlteration(string name)
            : base(name)
        { }

        public ColumnToAddBuilder AddColumn(Expression<Func<T, object>> propertyExpression, IDbType columnType)
        {
            return AddColumn(propertyExpression.GetMemberName(), columnType);
        }
    }

}
