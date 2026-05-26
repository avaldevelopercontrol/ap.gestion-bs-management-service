
using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class ProductTests
    {
        [Fact]
        public void Product_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var product = new Product
            {
                ProductId = 1,
                MerchantId = 100,
                CuentaMerchant = "ACC001",
                Code = "PROD001",
                Name = "Premium Subscription",
                Amount = 99.99m,
                FreqIntvId = 1,
                FreqIntvCount = 1,
                ChargeTypeId = 1,
                IsActive = true,
                IsDeleted = false
            };

            // Assert
            Assert.Equal(1, product.ProductId);
            Assert.Equal(100, product.MerchantId);
            Assert.Equal("ACC001", product.CuentaMerchant);
            Assert.Equal("PROD001", product.Code);
            Assert.Equal("Premium Subscription", product.Name);
            Assert.Equal(99.99m, product.Amount);
            Assert.Equal(1, product.FreqIntvId);
            Assert.Equal(1, product.FreqIntvCount);
            Assert.Equal(1, product.ChargeTypeId);
            Assert.True(product.IsActive);
            Assert.False(product.IsDeleted);
        }

        [Fact]
        public void Product_ShouldAllowSubscriptionsCollection()
        {
            // Arrange
            var product = new Product();
            var subscriptions = new List<Subscription>();

            // Act
            product.suscriptions = subscriptions;

            // Assert
            Assert.NotNull(product.suscriptions);
            Assert.Empty(product.suscriptions);
        }

        [Fact]
        public void Product_ShouldHandleDeletedProduct()
        {
            // Arrange & Act
            var product = new Product
            {
                ProductId = 1,
                Name = "Deleted Product",
                IsActive = false,
                IsDeleted = true
            };

            // Assert
            Assert.False(product.IsActive);
            Assert.True(product.IsDeleted);
        }

        [Fact]
        public void Product_ShouldHandleZeroAmount()
        {
            // Arrange & Act
            var product = new Product
            {
                ProductId = 1,
                Name = "Free Product",
                Amount = 0m
            };

            // Assert
            Assert.Equal(0m, product.Amount);
        }

        [Fact]
        public void Product_ShouldAllowEmptyValues()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            Assert.Equal(0, product.ProductId);
            Assert.Equal(0, product.MerchantId);
            Assert.Null(product.CuentaMerchant);
            Assert.Null(product.Code);
            Assert.Null(product.Name);
            Assert.Equal(0m, product.Amount);
            Assert.False(product.IsActive);
            Assert.False(product.IsDeleted);
        }
    }
}
