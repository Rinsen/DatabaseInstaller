using Rinsen.DatabaseInstaller.SqlTypes;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class ClusteredIndexTests
    {
        [Fact]
        public void GetCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new ClusteredIndex("MyIndex", "MyTable");
            index.AddColumn("MyColumn");

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE CLUSTERED INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }


    }
}
