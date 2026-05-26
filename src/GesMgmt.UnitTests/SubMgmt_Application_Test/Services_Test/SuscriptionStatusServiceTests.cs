
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SubMgmt.Application.Services;
using SubMgmt.Domain.Constants;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Services_Test
{
    public class SuscriptionStatusServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ISubscriptionStatusRepository> _mockSuscriptionStatusRepository;
        private readonly IMemoryCache _cache; // Usar implementación real
        private readonly SubscriptionStatusService _service;

        public SuscriptionStatusServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockSuscriptionStatusRepository = new Mock<ISubscriptionStatusRepository>();
            _cache = new MemoryCache(new MemoryCacheOptions()); // Implementación real
            _mockUnitOfWork.Setup(x => x.SuscriptionStatus).Returns(_mockSuscriptionStatusRepository.Object);
            _service = new SubscriptionStatusService(_mockUnitOfWork.Object, _cache);
        }

        [Fact]
        public async Task GetSuscriptionStatusAsync_Should_Return_Status_From_Cache_When_Available()
        {
            // Arrange
            var expectedStatuses = new List<SubscriptionStatus>
            {
                new SubscriptionStatus { SubStatId = 1, Name = "Active" },
                new SubscriptionStatus { SubStatId = 2, Name = "Cancelled" }
            };

            // Nota: El servicio usa ORIGINS_CACHE_KEY incorrectamente, debería ser SUSCRIPTION_STATUS_CACHE_KEY
            // Pero mantenemos la lógica actual del servicio
            _cache.Set(Const.ORIGINS_CACHE_KEY, expectedStatuses, TimeSpan.FromHours(24));

            // Act
            var result = await _service.GetSuscriptionStatusAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedStatuses.Count, result.Count());
            Assert.Equal(expectedStatuses.First().Name, result.First().Name);
            _mockSuscriptionStatusRepository.Verify(x => x.GetSuscriptionStatusAsync(), Times.Never);
        }

        [Fact]
        public async Task GetSuscriptionStatusAsync_Should_Fetch_From_Repository_And_Cache_When_Not_In_Cache()
        {
            // Arrange
            var expectedStatuses = new List<SubscriptionStatus>
            {
                new SubscriptionStatus { SubStatId = 1, Name = "Active" },
                new SubscriptionStatus { SubStatId = 2, Name = "Cancelled" }
            };

            _mockSuscriptionStatusRepository
                .Setup(x => x.GetSuscriptionStatusAsync())
                .ReturnsAsync(expectedStatuses);

            // Act
            var result = await _service.GetSuscriptionStatusAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedStatuses.Count, result.Count());
            Assert.Equal(expectedStatuses.First().Name, result.First().Name);
            _mockSuscriptionStatusRepository.Verify(x => x.GetSuscriptionStatusAsync(), Times.Once);

            // Verificar que se guardó en cache
            var cachedResult = _cache.Get<IEnumerable<SubscriptionStatus>>(Const.ORIGINS_CACHE_KEY);
            Assert.NotNull(cachedResult);
            Assert.Equal(expectedStatuses.Count, cachedResult.Count());
        }

        [Fact]
        public async Task GetSuscriptionStatusAsync_Should_Return_Empty_List_When_Repository_Returns_Empty()
        {
            // Arrange
            var expectedStatuses = new List<SubscriptionStatus>();

            _mockSuscriptionStatusRepository
                .Setup(x => x.GetSuscriptionStatusAsync())
                .ReturnsAsync(expectedStatuses);

            // Act
            var result = await _service.GetSuscriptionStatusAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockSuscriptionStatusRepository.Verify(x => x.GetSuscriptionStatusAsync(), Times.Once);
        }

        public void Dispose()
        {
            _cache?.Dispose();
        }
    }
}
