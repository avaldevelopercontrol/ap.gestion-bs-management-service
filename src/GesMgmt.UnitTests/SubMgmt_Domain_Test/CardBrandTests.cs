
using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class CardBrandTests
    {
        [Fact]
        public void CardBrand_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var cardBrand = new CardBrand
            {
                CardBrandId = 1,
                Brand = "VISA",
                Description = "Visa Card"
            };

            // Assert
            Assert.Equal(1, cardBrand.CardBrandId);
            Assert.Equal("VISA", cardBrand.Brand);
            Assert.Equal("Visa Card", cardBrand.Description);
        }

        [Fact]
        public void CardBrand_ShouldAllowEmptyValues()
        {
            // Arrange & Act
            var cardBrand = new CardBrand();

            // Assert
            Assert.Equal(0, cardBrand.CardBrandId);
            Assert.Null(cardBrand.Brand);
            Assert.Null(cardBrand.Description);
        }

    }
}
