

using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class MovementTests
    {

        [Fact]
        public void Movement_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var movement = new Movement();

            // Assert
            Assert.IsAssignableFrom<BaseEntity>(movement);
        }

        [Fact]
        public void Movement_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var movement = new Movement
            {
                MovementId = 1,
                LotId = 100,
                LotNumber = "LOT001",
                MerchantId = 1,
                CuentaMerchant = "ACC001",
                SuscriptionId = 1,
                Correlative = 1,
                PayMethodId = "PM001",
                BuyerId = 1,
                MovStatId = 1,
                MerchantBuyerId = "MB001",
                CardToken = "token123",
                CardBrand = "VISA",
                CurrencyCode = "USD",
                TotalAmount = 100.50m,
                NextPaymentDate = DateTime.Now,
                FlagRetry = true,
                RetryNumber = 1,
                RetryNumberMax = 3,
                ChargeTypeId = 1
            };

            // Assert
            Assert.Equal(1, movement.MovementId);
            Assert.Equal(100, movement.LotId);
            Assert.Equal("LOT001", movement.LotNumber);
            Assert.Equal(1, movement.MerchantId);
            Assert.Equal("ACC001", movement.CuentaMerchant);
            Assert.Equal(1, movement.SuscriptionId);
            Assert.Equal(1, movement.Correlative);
            Assert.Equal("PM001", movement.PayMethodId);
            Assert.Equal(1, movement.BuyerId);
            Assert.Equal(1, movement.MovStatId);
            Assert.Equal("MB001", movement.MerchantBuyerId);
            Assert.Equal("token123", movement.CardToken);
            Assert.Equal("VISA", movement.CardBrand);
            Assert.Equal("USD", movement.CurrencyCode);
            Assert.Equal(100.50m, movement.TotalAmount);
            Assert.True(movement.FlagRetry);
            Assert.Equal(1, movement.RetryNumber);
            Assert.Equal(3, movement.RetryNumberMax);
            Assert.Equal(1, movement.ChargeTypeId);
        }

        [Fact]
        public void Movement_ShouldAllowNullableProperties()
        {
            // Arrange & Act
            var movement = new Movement();

            // Assert
            Assert.Null(movement.PayMethodId);
            Assert.Null(movement.BuyerId);
            Assert.Null(movement.TotalAmount);
            Assert.Null(movement.NextPaymentDate);
        }

    }
}
