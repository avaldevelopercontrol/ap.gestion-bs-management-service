
using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class BuyerTests
    {
        [Fact]
        public void Buyer_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var buyer = new Buyer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                MerchantBuyerId = "MB123",
                Estado = 1,
                Type = 2,
                DocType = "DNI",
                DocNumber = "12345678",
                MerchantId = "M001",
                PhoneNumber = "123456789"
            };

            // Assert
            Assert.Equal(1, buyer.Id);
            Assert.Equal("John", buyer.FirstName);
            Assert.Equal("Doe", buyer.LastName);
            Assert.Equal("john.doe@email.com", buyer.Email);
            Assert.Equal("MB123", buyer.MerchantBuyerId);
            Assert.Equal(1, buyer.Estado);
            Assert.Equal(2, buyer.Type);
            Assert.Equal("DNI", buyer.DocType);
            Assert.Equal("12345678", buyer.DocNumber);
            Assert.Equal("M001", buyer.MerchantId);
            Assert.Equal("123456789", buyer.PhoneNumber);
        }

        [Fact]
        public void Buyer_ShouldInitializeCollections()
        {
            // Arrange & Act
            var buyer = new Buyer();

            // Assert
            Assert.Null(buyer.suscriptions);
            Assert.Null(buyer.BuyerCards);
        }

        [Fact]
        public void Buyer_ShouldAllowCollectionAssignment()
        {
            // Arrange
            var buyer = new Buyer();
            var subscriptions = new List<Subscription>();
            var buyerCards = new List<BuyerCard>();

            // Act
            buyer.suscriptions = subscriptions;
            buyer.BuyerCards = buyerCards;

            // Assert
            Assert.NotNull(buyer.suscriptions);
            Assert.NotNull(buyer.BuyerCards);
            Assert.Empty(buyer.suscriptions);
            Assert.Empty(buyer.BuyerCards);
        }

    }
}
