
using SubMgmt.Application.DTOs;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.DTOs_Test
{
    public class GetSubscriptionRequestDtoTests
    {

        [Fact]
        public void GetSubscriptionRequestDto_Default_Values()
        {
            // Arrange & Act
            var request = new GetSubscriptionRequestDto();

            // Assert
            Assert.Equal(string.Empty, request.MerchantCode);
            Assert.Equal(string.Empty, request.Language);
            Assert.Equal(string.Empty, request.Search);
            Assert.Equal(0, request.SubStatId);
            Assert.Equal(0, request.OriginId);
            Assert.Equal(1, request.PageNumber);
            Assert.Equal(10, request.PageSize);
        }

        [Fact]
        public void GetSubscription_RequestDto_Limit_PageSize_To_Maximum_50()
        {
            // Arrange
            var request = new GetSubscriptionRequestDto();

            // Act
            request.PageSize = 100; // Intentar establecer más del máximo

            // Assert
            Assert.Equal(50, request.PageSize); // Debe limitarse a 50
        }

        [Fact]
        public void GetSubscriptionRequestDto_Valid_PageSize_Within_Limit()
        {
            // Arrange
            var request = new GetSubscriptionRequestDto();

            // Act
            request.PageSize = 25;

            // Assert
            Assert.Equal(25, request.PageSize);
        }

    }
}
