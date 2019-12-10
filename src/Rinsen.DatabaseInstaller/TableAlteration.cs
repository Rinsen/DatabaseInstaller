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

        public ColumnBuilder AlterColumn(Expression<Func<T, object>> propertyExpression, int? length = null, int? precision = null)
        {
            var propertyType = propertyExpression.GetMemberType();
            var columnTypeResult = GetColumnTypeResult(propertyType, length, precision);

            var columnBuilder = AlterColumn(propertyExpression, columnTypeResult.DbType);

            if (columnTypeResult.Null)
            {
                columnBuilder.Null();
            }

            return columnBuilder;
        }

        public ColumnBuilder AlterColumn(Expression<Func<T, object>> propertyExpression, IDbType dbType)
        {
            
            var name = propertyExpression.GetMemberName();

            return AlterColumn(name, dbType);
        }

        private ColumnBuilder AlterColumn(string name, IDbType dbType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (ColumnsToAlter.Any(col => col.Name == name))
            {
                throw new ArgumentException($"A column with the name {name} already exist in table alteration {Name}");
            }

            var columnBuilder = new ColumnBuilder(this, name, dbType);

            ColumnsToAlter.Add(columnBuilder.Column);

            return columnBuilder;
        }
    }

    public class TableAlteration : Table
    {

        public TableAlteration(string name)
            :base(name, true)
        {
        }

        public ColumnBuilder AlterColumn(string name, IDbType dbType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (ColumnsToAlter.Any(col => col.Name == name))
            {
                throw new ArgumentException($"A column with the name {name} already exist in table alteration {Name}");
            }

            var columnBuilder = new ColumnBuilder(this, name, dbType);

            ColumnsToAlter.Add(columnBuilder.Column);

            return columnBuilder;
        }

        public void DropColumn(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is mandatory for column");
            }

            if (ColumnsToDrop.Any(col => col == name))
            {
                throw new ArgumentException($"A column with the name {name} already exist in table alteration {Name}");
            }

            ColumnsToDrop.Add(name);
        }
    }
}
