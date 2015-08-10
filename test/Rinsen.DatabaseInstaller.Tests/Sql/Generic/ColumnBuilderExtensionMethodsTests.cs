using Rinsen.DatabaseInstaller.Sql.Generic;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql.Generic
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
            Assert.Equal(1, table.Columns.Where(m => m.ForeignKey != null).Count());
            Assert.Equal("ClubId", table.Columns.First(m => m.ForeignKey != null).ForeignKey.ColumnName);
            Assert.Equal("Clubs", table.Columns.First(m => m.ForeignKey != null).ForeignKey.TableName);
        }

    }
}
