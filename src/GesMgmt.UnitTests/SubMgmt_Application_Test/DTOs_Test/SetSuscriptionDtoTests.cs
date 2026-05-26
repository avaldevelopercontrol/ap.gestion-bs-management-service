
using SubMgmt.Application.DTOs;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.DTOs_Test
{
    public class SetSuscriptionDtoTests
    {
        [Fact]
        public void SetSuscriptionDto_Initialize_With_Default_Values()
        {
            // Arrange & Act
            var dto = new SetSuscriptionDto();

            // Assert
            Assert.Equal(string.Empty, dto.MerchantCode);
            Assert.Equal(string.Empty, dto.Language);
            Assert.Equal(string.Empty, dto.Searcher);
            Assert.Equal(0, dto.SubStatId);
            Assert.Equal(0, dto.OriginId);
            Assert.Equal(1, dto.PageNumber);
            Assert.Equal(10, dto.PageSize);
        }

        [Fact]
        public void SetSuscriptionDto_Should_Limit_PageSize_To_Maximum_50()
        {
            // Arrange
            var dto = new SetSuscriptionDto();

            // Act
            dto.PageSize = 75; // Intentar establecer más del máximo

            // Assert
            Assert.Equal(50, dto.PageSize); // Debe limitarse a 50
        }

        [Fact]
        public void SetSuscriptionDto_Allow_Valid_PageSize_Within_Limit()
        {
            // Arrange
            var dto = new SetSuscriptionDto();

            // Act
            dto.PageSize = 30;

            // Assert
            Assert.Equal(30, dto.PageSize);
        }

    }
}
