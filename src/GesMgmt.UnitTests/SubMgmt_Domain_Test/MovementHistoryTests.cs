
using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class MovementHistoryTests
    {
        [Fact]
        public void MovementHistory_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var movementHistory = new MovementHistory
            {
                MovementHistoryId = 1,
                MovementId = 100,
                CreatedAt = DateTime.Now,
                MovStatId = 1,
                ResponseCode = "200",
                MessageResponse = "Success"
            };

            // Assert
            Assert.Equal(1, movementHistory.MovementHistoryId);
            Assert.Equal(100, movementHistory.MovementId);
            Assert.True(movementHistory.CreatedAt <= DateTime.Now);
            Assert.Equal(1, movementHistory.MovStatId);
            Assert.Equal("200", movementHistory.ResponseCode);
            Assert.Equal("Success", movementHistory.MessageResponse);
        }

        [Fact]
        public void MovementHistory_ShouldAllowEmptyValues()
        {
            // Arrange & Act
            var movementHistory = new MovementHistory();

            // Assert
            Assert.Equal(0, movementHistory.MovementHistoryId);
            Assert.Equal(0, movementHistory.MovementId);
            Assert.Equal(default(DateTime), movementHistory.CreatedAt);
            Assert.Equal(0, movementHistory.MovStatId);
            Assert.Null(movementHistory.ResponseCode);
            Assert.Null(movementHistory.MessageResponse);
        }

        [Fact]
        public void MovementHistory_ShouldHandleSpecialCharactersInResponse()
        {
            // Arrange & Act
            var movementHistory = new MovementHistory
            {
                ResponseCode = "ERR-001",
                MessageResponse = "Error: Invalid card número"
            };

            // Assert
            Assert.Equal("ERR-001", movementHistory.ResponseCode);
            Assert.Equal("Error: Invalid card número", movementHistory.MessageResponse);
        }
    }
}
