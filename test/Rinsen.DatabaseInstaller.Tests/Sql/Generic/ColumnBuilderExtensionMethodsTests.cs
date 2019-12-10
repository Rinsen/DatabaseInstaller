using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Generic
{
    public class ColumnBuilderExtensionMethodsTests
    {
        public class Club
        {
            public int ClubId { get; set; }
        }

        public class ClubMember
        {
            public int Id { get; set; }

            public int ClubId { get; set; }

        }

        [Fact]
        public void AddForeignKeyConstraintOneOneColumnWithGenericExtensionMethod_GetCorrectInformationInTable()
        {
            // Arrange
            var table = new Table<ClubMember>("ClubMembers");

            // Act
            table.AddAutoIncrementColumn(m => m.Id);
            table.AddColumn(m => m.ClubId).ForeignKey<Club>(m => m.ClubId);

            // Assert
            Assert.Single(table.ColumnsToAdd.Where(m => m.ForeignKey != null));
            Assert.Equal("ClubId", table.ColumnsToAdd.First(m => m.ForeignKey != null).ForeignKey.ColumnName);
            Assert.Equal("Clubs", table.ColumnsToAdd.First(m => m.ForeignKey != null).ForeignKey.TableName);
        }

    }
}
