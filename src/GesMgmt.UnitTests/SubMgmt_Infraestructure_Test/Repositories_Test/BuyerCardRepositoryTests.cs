using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Infraestructure_Test.Repositories_Test
{
    public class BuyerCardRepositoryTests
    {

        [Fact]
        public void BuyerCard_Should_Set_Properties_Correctly()
        {
            // Arrange & Act
            var buyerCard = new BuyerCard
            {
                BuyerId = 1,
                CardToken = "TOKEN123",
                Estado = 1
            };

            // Assert
            Assert.Equal(1, buyerCard.BuyerId);
            Assert.Equal("TOKEN123", buyerCard.CardToken);
            Assert.Equal(1, buyerCard.Estado);
        }

        [Fact]
        public void BuyerCard_Should_Allow_Different_Estados()
        {
            // Arrange & Act
            var activeBuyerCard = new BuyerCard { Estado = 1 };
            var inactiveBuyerCard = new BuyerCard { Estado = 0 };

            // Assert
            Assert.Equal(1, activeBuyerCard.Estado);
            Assert.Equal(0, inactiveBuyerCard.Estado);
        }

        [Fact]
        public void BuyerCard_Should_Handle_Null_CardToken()
        {
            // Arrange & Act
            var buyerCard = new BuyerCard
            {
                BuyerId = 1,
                CardToken = null,
                Estado = 1
            };

            // Assert
            Assert.Equal(1, buyerCard.BuyerId);
            Assert.Null(buyerCard.CardToken);
        }
    }
}
