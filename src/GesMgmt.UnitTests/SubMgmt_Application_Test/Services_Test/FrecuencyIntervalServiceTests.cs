using Moq;
using SubMgmt.Application.Services;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Services_Test
{
    public class FrecuencyIntervalServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IFrecuencyIntervalRepository> _mockFrecuencyIntervalRepository;
        private readonly FrecuencyIntervalService _service;

        public FrecuencyIntervalServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockFrecuencyIntervalRepository = new Mock<IFrecuencyIntervalRepository>();
            _mockUnitOfWork.Setup(x => x.FrecuencyIntervals).Returns(_mockFrecuencyIntervalRepository.Object);
            _service = new FrecuencyIntervalService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetFrecuencyIntervalsAsync_Return_FrecuencyIntervals_From_Repository()
        {
            // Arrange
            var expectedIntervals = new List<FrecuencyInterval>
            {
                new FrecuencyInterval { FreqIntvId = 1, Name = "Weekly" },
                new FrecuencyInterval { FreqIntvId = 2, Name = "Monthly" }
            }.AsQueryable();

            _mockFrecuencyIntervalRepository
                .Setup(x => x.GetFrecuencyIntervalsAsync())
                .ReturnsAsync(expectedIntervals);

            // Act
            var result = await _service.GetFrecuencyIntervalsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedIntervals, result);
            _mockFrecuencyIntervalRepository.Verify(x => x.GetFrecuencyIntervalsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetFrecuencyIntervalsAsync_Call_Repository_Once()
        {
            // Arrange
            var expectedIntervals = new List<FrecuencyInterval>().AsQueryable();
            _mockFrecuencyIntervalRepository
                .Setup(x => x.GetFrecuencyIntervalsAsync())
                .ReturnsAsync(expectedIntervals);

            // Act
            await _service.GetFrecuencyIntervalsAsync();

            // Assert
            _mockFrecuencyIntervalRepository.Verify(x => x.GetFrecuencyIntervalsAsync(), Times.Once);
        }

    }
}
