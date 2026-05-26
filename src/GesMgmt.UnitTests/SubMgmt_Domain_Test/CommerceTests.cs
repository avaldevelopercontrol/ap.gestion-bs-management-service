
using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class CommerceTests
    {
        [Fact]
        public void Commerce_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var commerce = new Commerce
            {
                Merchant_Id = 1,
                CuentaMerchant = "ACC001",
                NombreMerchant = "Test Merchant"
            };

            // Assert
            Assert.Equal(1, commerce.Merchant_Id);
            Assert.Equal("ACC001", commerce.CuentaMerchant);
            Assert.Equal("Test Merchant", commerce.NombreMerchant);
        }

    }
}
