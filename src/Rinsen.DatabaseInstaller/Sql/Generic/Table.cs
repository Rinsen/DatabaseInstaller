﻿using System;
using System.Linq.Expressions;

namespace Rinsen.DatabaseInstaller.Sql.Generic
{
    public class Table<T> : Table
    {
        public Table(string name)
            : base(name)
        { }

        public ColumnBuilder AddColumn(Expression<Func<T, object>> propertyExpression, IDbType columnType)
        {
            var name = propertyExpression.GetMemberName();

            return AddColumn(name, columnType);
        }

        
    }
}