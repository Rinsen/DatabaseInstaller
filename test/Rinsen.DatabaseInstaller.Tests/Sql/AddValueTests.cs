using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class AddValueTests
    {
        [Fact]
        public void AddValueToNullColumnToBeAbleToAddNotNullConstraint()
        {
            var addValue = new AddValue("MyTable");

            addValue.GuidColumn("ColumnName");

            var script = addValue.GetUpScript().Single();

            Assert.Equal("UPDATE MyTable\r\nSET ColumnName = NEWID()\r\nWHERE ColumnName is NULL", script);
        }
    }
}
