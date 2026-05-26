

using SubMgmt.Application.DTOs;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.DTOs_Test
{
    public class ResultListDtoTests
    {
        [Fact]
        public void ResultListDto_Success_Create_Successful_List_Result()
        {
            // Arrange
            var responseData = new List<string> { "Item1", "Item2" };
            var code = "SUCCESS";
            var message = "List retrieved successfully";
            var messageUser = "Lista obtenida exitosamente";
            var statusCode = 200;

            // Act
            var result = ResultListDto<List<string>>.Success(responseData, code, message, messageUser, statusCode);

            // Assert
            Assert.Equal(responseData, result.Response);
            Assert.Equal(code, result.Code);
            Assert.Equal(message, result.Message);
            Assert.Equal(messageUser, result.MessageUser);
            Assert.Equal(statusCode, result.StatusCode);
        }

        [Fact]
        public void ResultListDto_Allow_Setting_Pagination_Properties()
        {
            // Arrange
            var result = ResultListDto<List<string>>.Success(
                new List<string>(), "SUCCESS", "OK", "OK", 200);

            // Act
            result.PageNumber = 2;
            result.PageSize = 20;
            result.TotalRecords = 100;
            result.TotalPages = 5;

            // Assert
            Assert.Equal(2, result.PageNumber);
            Assert.Equal(20, result.PageSize);
            Assert.Equal(100, result.TotalRecords);
            Assert.Equal(5, result.TotalPages);
        }

        [Fact]
        public void ResultListDto_Create_Failed_Result_With_Default_Pagination()
        {
            // Arrange & Act
            var result = ResultListDto<List<string>>.Failure("ERROR", "Failed", "Falló", 400);

            // Assert
            Assert.Null(result.Response);
            Assert.Equal("ERROR", result.Code);
            Assert.Equal(0, result.PageNumber); // Default value
            Assert.Equal(0, result.PageSize); // Default value
            Assert.Equal(0, result.TotalRecords); // Default value
            Assert.Equal(0, result.TotalPages); // Default value
        }

    }
}
