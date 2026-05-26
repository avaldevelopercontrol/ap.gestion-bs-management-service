

using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class MovStatTests
    {
        [Fact]
        public void MovStat_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var movStat = new MovStat
            {
                MovStatId = 1,
                Name = "Pending",
                IsActive = true
            };

            // Assert
            Assert.Equal(1, movStat.MovStatId);
            Assert.Equal("Pending", movStat.Name);
            Assert.True(movStat.IsActive);
        }

        [Fact]
        public void MovStat_ShouldAllowMovementsCollection()
        {
            // Arrange
            var movStat = new MovStat();
            var movements = new List<Movement>();

            // Act
            movStat.movements = movements;

            // Assert
            Assert.NotNull(movStat.movements);
            Assert.Empty(movStat.movements);
        }

        [Fact]
        public void MovStat_ShouldHandleInactiveStatus()
        {
            // Arrange & Act
            var movStat = new MovStat
            {
                MovStatId = 2,
                Name = "Cancelled",
                IsActive = false
            };

            // Assert
            Assert.Equal(2, movStat.MovStatId);
            Assert.Equal("Cancelled", movStat.Name);
            Assert.False(movStat.IsActive);
        }

        [Fact]
        public void MovStat_ShouldAllowEmptyValues()
        {
            // Arrange & Act
            var movStat = new MovStat();

            // Assert
            Assert.Equal(0, movStat.MovStatId);
            Assert.Null(movStat.Name);
            Assert.False(movStat.IsActive);
            Assert.Null(movStat.movements);
        }
    }
}
