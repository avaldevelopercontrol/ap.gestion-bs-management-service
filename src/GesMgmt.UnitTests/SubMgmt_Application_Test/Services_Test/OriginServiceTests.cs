
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SubMgmt.Application.Services;
using SubMgmt.Domain.Constants;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Services_Test
{
    public class OriginServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOriginRepository> _mockOriginRepository;
        private readonly IMemoryCache _cache; 
        private readonly OriginService _service;

        public OriginServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOriginRepository = new Mock<IOriginRepository>();
            _cache = new MemoryCache(new MemoryCacheOptions()); 
            _mockUnitOfWork.Setup(x => x.Origins).Returns(_mockOriginRepository.Object);
            _service = new OriginService(_mockUnitOfWork.Object, _cache);
        }

        [Fact]
        public async Task GetOriginsAsync_Should_Return_Origins_From_Cache_When_Available()
        {
            // Arrange
            var expectedOrigins = new List<Origin>
            {
                new Origin { OriginId = 1, Name = "API" },
                new Origin { OriginId = 2, Name = "WEB" }
            };

            // Precargar el cache
            _cache.Set(Const.ORIGINS_CACHE_KEY, expectedOrigins, TimeSpan.FromHours(24));

            // Act
            var result = await _service.GetOriginsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrigins.Count, result.Count());
            Assert.Equal(expectedOrigins.First().Name, result.First().Name);
            _mockOriginRepository.Verify(x => x.GetOriginsAsync(), Times.Never);
        }

        [Fact]
        public async Task GetOriginsAsync_Should_Fetch_From_Repository_And_Cache_When_Not_In_Cache()
        {
            // Arrange
            var expectedOrigins = new List<Origin>
            {
                new Origin { OriginId = 1, Name = "API" },
                new Origin { OriginId = 2, Name = "WEB" }
            };

            _mockOriginRepository
                .Setup(x => x.GetOriginsAsync())
                .ReturnsAsync(expectedOrigins);

            // Act
            var result = await _service.GetOriginsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrigins.Count, result.Count());
            Assert.Equal(expectedOrigins.First().Name, result.First().Name);
            _mockOriginRepository.Verify(x => x.GetOriginsAsync(), Times.Once);

            // Verificar que se guardó en cache
            var cachedResult = _cache.Get<IEnumerable<Origin>>(Const.ORIGINS_CACHE_KEY);
            Assert.NotNull(cachedResult);
            Assert.Equal(expectedOrigins.Count, cachedResult.Count());
        }

        [Fact]
        public async Task GetOriginsAsync_Should_Cache_For_24_Hours()
        {
            // Arrange
            var expectedOrigins = new List<Origin>
            {
                new Origin { OriginId = 1, Name = "API" }
            };

            _mockOriginRepository
                .Setup(x => x.GetOriginsAsync())
                .ReturnsAsync(expectedOrigins);

            // Act
            await _service.GetOriginsAsync();

            // Assert
            var cachedResult = _cache.Get<IEnumerable<Origin>>(Const.ORIGINS_CACHE_KEY);
            Assert.NotNull(cachedResult);
            Assert.Equal(expectedOrigins.Count, cachedResult.Count());

            // Verificar que el repositorio fue llamado
            _mockOriginRepository.Verify(x => x.GetOriginsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetOriginsAsync_Should_Return_Empty_List_When_Repository_Returns_Empty()
        {
            // Arrange
            var expectedOrigins = new List<Origin>();

            _mockOriginRepository
                .Setup(x => x.GetOriginsAsync())
                .ReturnsAsync(expectedOrigins);

            // Act
            var result = await _service.GetOriginsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockOriginRepository.Verify(x => x.GetOriginsAsync(), Times.Once);
        }

        public void Dispose()
        {
            _cache?.Dispose();
        }
    }
}
