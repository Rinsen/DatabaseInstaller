using Rinsen.DatabaseInstaller.SqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Rinsen.DatabaseInstaller
{
    public class TableAlteration<T> : Table<T>
    {
        public TableAlteration(string name)
            : base(name, true)
        {
            
        }

        public void DropColumn(Expression<Func<T, object>> propertyExpression)
        {
            var name = propertyExpression.GetMemberName();

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (ColumnsToDrop.Any(col => col == name))
            {
                throw new ArgumentException(string.Format("A column with the name {0} already exist in table alteration {1}", name, Name));
            }

            ColumnsToDrop.Add(name);
        }

    }

    public class TableAlteration : Table
    {

        public TableAlteration(string name)
            :base(name, true)
        {
        }

        public void DropColumn(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (ColumnsToDrop.Any(col => col == name))
            {
                throw new ArgumentException(string.Format("A column with the name {0} already exist in table alteration {1}", name, Name));
            }

            ColumnsToDrop.Add(name);
        }
    }
}
