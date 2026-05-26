
using Microsoft.EntityFrameworkCore;
using SubMgmt.Domain.Entities;
using SubMgmt.Infraestructure.Persistence;
using SubMgmt.Infraestructure.Repositories;

namespace SubMgmt.UnitTests.SubMgmt_Infraestructure_Test.Repositories_Test
{
    public class BuyerRepositoryTests
    {
        private RecurrenceDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RecurrenceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new RecurrenceDbContext(options);
        }

      
        [Fact]
        public async Task GetBuyerByIdAsync_Should_Return_Null_When_Not_Exists()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var repository = new BuyerRepository(context);

            // Act
            var result = await repository.GetBuyerByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Query_Should_Return_IQueryable()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var repository = new BuyerRepository(context);

            // Act
            var result = await repository.Query();

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IQueryable<Buyer>>(result);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Updated_Entity()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var repository = new BuyerRepository(context);

            var buyer = new Buyer
            {
                Id = 1,
                FirstName = "Juan",
                Estado = 1
            };

            // Act
            var result = await repository.UpdateAsync(buyer);

            // Assert
            Assert.Equal(buyer, result);
        }
    }
}
