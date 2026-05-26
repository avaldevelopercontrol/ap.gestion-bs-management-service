using Microsoft.EntityFrameworkCore;
using SubMgmt.Domain.Entities;
using SubMgmt.Infraestructure.Persistence;
using SubMgmt.Infraestructure.Repositories;

namespace SubMgmt.UnitTests.SubMgmt_Infraestructure_Test.Repositories_Test
{
    public class CommerceRepositoryTests
    {
        private RecurrenceDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RecurrenceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new RecurrenceDbContext(options);
        }

        [Fact]
        public async Task GetByCuentaMerchantAsync_Should_Return_Null_When_Not_Exists()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var repository = new CommerceRepository(context);

            // Act
            var result = await repository.GetByCuentaMerchantAsync("NONEXISTENT");

            // Assert
            Assert.Null(result);
        }
    }
}
