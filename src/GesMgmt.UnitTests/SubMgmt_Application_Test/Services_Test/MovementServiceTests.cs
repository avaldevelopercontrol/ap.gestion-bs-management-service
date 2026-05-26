
using Moq;
using SubMgmt.Application.Services;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Services_Test
{
    public class MovementServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMovementRepository> _mockMovementRepository;
        private readonly MovementService _service;

        public MovementServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMovementRepository = new Mock<IMovementRepository>();
            _mockUnitOfWork.Setup(x => x.Movements).Returns(_mockMovementRepository.Object);
            _service = new MovementService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task MovementsAsync_Movements_From_Repository()
        {
            // Arrange
            var expectedMovements = new List<Movement>
            {
                new Movement { MovementId = 1, SuscriptionId = 1 },
                new Movement { MovementId = 2, SuscriptionId = 2 }
            }.AsQueryable();

            _mockMovementRepository
                .Setup(x => x.GetMovementsAsync())
                .ReturnsAsync(expectedMovements);

            // Act
            var result = await _service.GetMovementsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMovements, result);
            _mockMovementRepository.Verify(x => x.GetMovementsAsync(), Times.Once);
        }

        [Fact]
        public async Task MovementBySubscriptionIdAsync_Return_Movement_When_Exists()
        {
            // Arrange
            var suscriptionId = 123;
            var expectedMovement = new Movement { MovementId = 1, SuscriptionId = suscriptionId };

            _mockMovementRepository
                .Setup(x => x.GetMovementBySubscriptionIdAsync(suscriptionId))
                .ReturnsAsync(expectedMovement);

            // Act
            var result = await _service.GetMovementBySubscriptionIdAsync(suscriptionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMovement, result);
            Assert.Equal(suscriptionId, result.SuscriptionId);
            _mockMovementRepository.Verify(x => x.GetMovementBySubscriptionIdAsync(suscriptionId), Times.Once);
        }

        [Fact]
        public async Task MovementBySubscriptionIdAsync_Return_Null_When_Not_Exists()
        {
            // Arrange
            var suscriptionId = 999;
            _mockMovementRepository
                .Setup(x => x.GetMovementBySubscriptionIdAsync(suscriptionId))
                .ReturnsAsync((Movement?)null);

            // Act
            var result = await _service.GetMovementBySubscriptionIdAsync(suscriptionId);

            // Assert
            Assert.Null(result);
            _mockMovementRepository.Verify(x => x.GetMovementBySubscriptionIdAsync(suscriptionId), Times.Once);
        }

        [Fact]
        public async Task MovementInLotRawBySubscriptionIdAsync_Return_Movements_In_Lot()
        {
            // Arrange
            var suscriptionId = 123;
            var expectedMovements = new List<Movement>
            {
                new Movement { MovementId = 1, SuscriptionId = suscriptionId, LotId = 1 }
            }.AsQueryable();

            _mockMovementRepository
                .Setup(x => x.GetMovementInLotRawBySubscriptionIdAsync(suscriptionId))
                .ReturnsAsync(expectedMovements);

            // Act
            var result = await _service.GetMovementInLotRawBySubscriptionIdAsync(suscriptionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMovements, result);
            _mockMovementRepository.Verify(x => x.GetMovementInLotRawBySubscriptionIdAsync(suscriptionId), Times.Once);
        }
    }
}
