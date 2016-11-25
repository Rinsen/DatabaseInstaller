using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Generic.Sql
{
    public class GenericUniqueClusteredIndexTests
    {
        class MyTable
        {
            public int MyColumn { get; set; }
        }

        [Fact]
        public void GetCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new UniqueClusteredIndex<MyTable>("MyIndex", "MyTable");
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Equal(1, createScripts.Count);
            Assert.Equal("CREATE UNIQUE CLUSTERED INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }
    }
}
