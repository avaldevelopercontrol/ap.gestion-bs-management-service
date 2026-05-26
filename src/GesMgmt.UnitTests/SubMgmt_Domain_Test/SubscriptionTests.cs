

using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class SubscriptionTests
    {
        [Fact]
        public void Subscription_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var subscription = new Subscription();

            // Assert
            Assert.IsAssignableFrom<BaseEntity>(subscription);
        }

        [Fact]
        public void Subscription_ShouldSetAndGetBasicProperties()
        {
            // Arrange & Act
            var subscription = new Subscription
            {
                SuscriptionId = 1,
                Code = "SUB001",
                MerchantId = 100,
                CuentaMerchant = "ACC001",
                BuyerId = 1,
                MerchantBuyerId = "MB001",
                CardToken = "token123",
                CardBrand = "VISA",
                PayMethodId = "PM001",
                ServiceCode = "SVC001"
            };

            // Assert
            Assert.Equal(1, subscription.SuscriptionId);
            Assert.Equal("SUB001", subscription.Code);
            Assert.Equal(100, subscription.MerchantId);
            Assert.Equal("ACC001", subscription.CuentaMerchant);
            Assert.Equal(1, subscription.BuyerId);
            Assert.Equal("MB001", subscription.MerchantBuyerId);
            Assert.Equal("token123", subscription.CardToken);
            Assert.Equal("VISA", subscription.CardBrand);
            Assert.Equal("PM001", subscription.PayMethodId);
            Assert.Equal("SVC001", subscription.ServiceCode);
        }

        [Fact]
        public void Subscription_ShouldSetAndGetAmountProperties()
        {
            // Arrange & Act
            var subscription = new Subscription
            {
                Amount = 100.50m,
                BaseAmount = 90.00m,
                MaximumAmount = 150.00m,
                Quantity = 2,
                TotalAmount = 201.00m
            };

            // Assert
            Assert.Equal(100.50m, subscription.Amount);
            Assert.Equal(90.00m, subscription.BaseAmount);
            Assert.Equal(150.00m, subscription.MaximumAmount);
            Assert.Equal(2, subscription.Quantity);
            Assert.Equal(201.00m, subscription.TotalAmount);
        }

        [Fact]
        public void Subscription_ShouldSetAndGetDateProperties()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddMonths(12);
            var nextPayment = DateTime.Now.AddDays(30);
            var confirmedAt = DateTime.Now;

            // Act
            var subscription = new Subscription
            {
                ContractStartDt = startDate,
                ContractEndDt = endDate,
                NextPaymentDate = nextPayment,
                ConfirmedAt = confirmedAt
            };

            // Assert
            Assert.Equal(startDate, subscription.ContractStartDt);
            Assert.Equal(endDate, subscription.ContractEndDt);
            Assert.Equal(nextPayment, subscription.NextPaymentDate);
            Assert.Equal(confirmedAt, subscription.ConfirmedAt);
        }

        [Fact]
        public void Subscription_ShouldAllowNavigationProperties()
        {
            // Arrange
            var buyer = new Buyer { Id = 1, FirstName = "John" };
            var product = new Product { ProductId = 1, Name = "Premium" };
            var chargeType = new ChargeType { ChargeTypeId = 1, Name = "Monthly" };
            var frecuencyInterval = new FrecuencyInterval { FreqIntvId = 1, Name = "Monthly" };
            var subscriptionStatus = new SubscriptionStatus { SubStatId = 1, Name = "Active" };
            var origin = new Origin { OriginId = 1, Name = "Web" };

            // Act
            var subscription = new Subscription
            {
                Buyer = buyer,
                Product = product,
                ChargeType = chargeType,
                FrecuencyInterval = frecuencyInterval,
                SuscriptionStatus = subscriptionStatus,
                Origin = origin
            };

            // Assert
            Assert.NotNull(subscription.Buyer);
            Assert.NotNull(subscription.Product);
            Assert.NotNull(subscription.ChargeType);
            Assert.NotNull(subscription.FrecuencyInterval);
            Assert.NotNull(subscription.SuscriptionStatus);
            Assert.NotNull(subscription.Origin);
        }

        [Fact]
        public void Subscription_ShouldHandleNullableProperties()
        {
            // Arrange & Act
            var subscription = new Subscription();

            // Assert
            Assert.Null(subscription.Code);
            Assert.Null(subscription.BuyerId);
            Assert.Null(subscription.MerchantBuyerId);
            Assert.Null(subscription.CardToken);
            Assert.Null(subscription.CardBrand);
            Assert.Null(subscription.PayMethodId);
            Assert.Null(subscription.ContractStartDt);
            Assert.Null(subscription.ContractEndDt);
            Assert.Null(subscription.NextPaymentDate);
            Assert.Null(subscription.InstalmentPlan);
            Assert.Null(subscription.CurrencyCode);
            Assert.Null(subscription.Amount);
            Assert.Null(subscription.BaseAmount);
            Assert.Null(subscription.MaximumAmount);
            Assert.Null(subscription.TotalAmount);
            Assert.Null(subscription.AddtlInf);
            Assert.Null(subscription.UserConfirmed);
            Assert.Null(subscription.ConfirmedAt);
            Assert.Null(subscription.isActNotif);
            Assert.Null(subscription.RejectionReason);
        }

        [Fact]
        public void Subscription_ShouldHandleDeletedStatus()
        {
            // Arrange & Act
            var subscription = new Subscription
            {
                IsDeleted = true,
                RejectionReason = "Invalid payment method"
            };

            // Assert
            Assert.True(subscription.IsDeleted);
            Assert.Equal("Invalid payment method", subscription.RejectionReason);
        }

    }
}
