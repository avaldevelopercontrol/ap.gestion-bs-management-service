
using Moq;
using SubMgmt.Application.Services;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Services_Test
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
            _service = new ProductService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetProductsAsync_Should_Return_Products_From_Repository()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { ProductId = 1, Name = "Premium Plan", Code = "PREM001" },
                new Product { ProductId = 2, Name = "Basic Plan", Code = "BASIC001" }
            }.AsQueryable();

            _mockProductRepository
                .Setup(x => x.GetProductsAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProducts, result);
            _mockProductRepository.Verify(x => x.GetProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_Should_Call_Repository_Once()
        {
            // Arrange
            var expectedProducts = new List<Product>().AsQueryable();
            _mockProductRepository
                .Setup(x => x.GetProductsAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            await _service.GetProductsAsync();

            // Assert
            _mockProductRepository.Verify(x => x.GetProductsAsync(), Times.Once);
        }
    }
}
