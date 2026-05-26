using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SubMgmt.Domain.Constants;
using SubMgmt.Domain.Entities;
using SubMgmt.Infraestructure.Persistence;
using SubMgmt.Infraestructure.Repositories;

namespace SubMgmt.UnitTests.SubMgmt_Infraestructure_Test.Repositories_Test
{
    public class CardBrandRepositoryTests
    {
        private RecurrenceDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RecurrenceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RecurrenceDbContext(options);
        }

        private IMemoryCache GetMemoryCache()
        {
            return new MemoryCache(new MemoryCacheOptions());
        }

      
        [Fact]
        public async Task GetByBrandAsync_WithInvalidBrand_ReturnsNull()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var cache = GetMemoryCache();
            var repository = new CardBrandRepository(context, cache);

            // Act
            var result = await repository.GetByBrandAsync("INVALID");

            // Assert
            Assert.Null(result);
        }
    }
}
