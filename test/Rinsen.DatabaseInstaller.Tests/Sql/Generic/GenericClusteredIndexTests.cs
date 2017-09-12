using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Generic.Sql
{
    public class GenericClusteredIndexTests
    {
        class MyTable
        {
            public int MyColumn { get; set; }
        }

        [Fact]
        public void GetCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new ClusteredIndex<MyTable>("MyIndex", "MyTable");
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE CLUSTERED INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }
    }
}
