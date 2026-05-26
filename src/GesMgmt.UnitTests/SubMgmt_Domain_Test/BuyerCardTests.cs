
using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class BuyerCardTests
    {
        [Fact]
        public void BuyerCard_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var buyerCard = new BuyerCard
            {
                Id = 1,
                BuyerId = 100,
                CardId = 200,
                CardToken = "token123",
                CardMask = "****1234",
                Estado = 1
            };

            // Assert
            Assert.Equal(1, buyerCard.Id);
            Assert.Equal(100, buyerCard.BuyerId);
            Assert.Equal(200, buyerCard.CardId);
            Assert.Equal("token123", buyerCard.CardToken);
            Assert.Equal("****1234", buyerCard.CardMask);
            Assert.Equal(1, buyerCard.Estado);
        }

        [Fact]
        public void BuyerCard_ShouldAllowBuyerNavigation()
        {
            // Arrange
            var buyer = new Buyer { Id = 100, FirstName = "John" };
            var buyerCard = new BuyerCard();

            // Act
            buyerCard.Buyer = buyer;

            // Assert
            Assert.NotNull(buyerCard.Buyer);
            Assert.Equal(100, buyerCard.Buyer.Id);
            Assert.Equal("John", buyerCard.Buyer.FirstName);
        }
    }
}
