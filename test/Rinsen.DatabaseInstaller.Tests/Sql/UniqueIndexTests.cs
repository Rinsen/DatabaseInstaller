using Rinsen.DatabaseInstaller.SqlTypes;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class UniqueIndexTests
    {
        [Fact]
        public void GetCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new UniqueIndex("MyIndex", "MyTable");
            index.AddColumn("MyColumn");

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE UNIQUE INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }

    }
}
