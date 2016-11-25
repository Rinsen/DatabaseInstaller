using Rinsen.DatabaseInstaller.SqlTypes;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Generic.Sql
{
    public class GenericUniqueIndexTests
    {
        class MyTable
        {
            public int MyColumn { get; set; }
        }

        [Fact]
        public void GetCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new UniqueIndex<MyTable>("MyIndex", "MyTable");
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Equal(1, createScripts.Count);
            Assert.Equal("CREATE UNIQUE INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }
    }
}
