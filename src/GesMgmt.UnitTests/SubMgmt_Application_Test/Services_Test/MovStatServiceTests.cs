
using Moq;
using SubMgmt.Application.Services;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Services_Test
{
    public class MovStatServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMovStatRepository> _mockMovStatRepository;
        private readonly MovStatService _service;

        public MovStatServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMovStatRepository = new Mock<IMovStatRepository>();
            _mockUnitOfWork.Setup(x => x.MovStats).Returns(_mockMovStatRepository.Object);
            _service = new MovStatService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetMovStatsAsync_Return_MovStats_From_Repository()
        {
            // Arrange
            var expectedMovStats = new List<MovStat>
            {
                new MovStat { MovStatId = 1, Name = "Pending" },
                new MovStat { MovStatId = 2, Name = "Processed" }
            }.AsQueryable();

            _mockMovStatRepository
                .Setup(x => x.GetMovStatsAsync())
                .ReturnsAsync(expectedMovStats);

            // Act
            var result = await _service.GetMovStatsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMovStats, result);
            _mockMovStatRepository.Verify(x => x.GetMovStatsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetMovStatsAsync_Call_Repository_Once()
        {
            // Arrange
            var expectedMovStats = new List<MovStat>().AsQueryable();
            _mockMovStatRepository
                .Setup(x => x.GetMovStatsAsync())
                .ReturnsAsync(expectedMovStats);

            // Act
            await _service.GetMovStatsAsync();

            // Assert
            _mockMovStatRepository.Verify(x => x.GetMovStatsAsync(), Times.Once);
        }
    }
}
