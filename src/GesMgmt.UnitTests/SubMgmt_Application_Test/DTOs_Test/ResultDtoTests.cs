
using SubMgmt.Application.DTOs;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.DTOs_Test
{
    public class ResultDtoTests
    {
        [Fact]
        public void ResultDto_Success_Create_Successful_Result_With_Data()
        {
            // Arrange
            var responseData = "Test Data";
            var code = "SUCCESS";
            var message = "Operation successful";
            var messageUser = "Operación exitosa";
            var statusCode = 200;

            // Act
            var result = ResultDto<string>.Success(responseData, code, message, messageUser, statusCode);

            // Assert
            Assert.Equal(responseData, result.Response);
            Assert.Equal(code, result.Code);
            Assert.Equal(message, result.Message);
            Assert.Equal(messageUser, result.MessageUser);
            Assert.Equal(statusCode, result.StatusCode);
        }

        [Fact]
        public void ResultDto_Failure_Create_Failed_Result_With_Default_Response()
        {
            // Arrange
            var code = "ERROR";
            var message = "Operation failed";
            var messageUser = "Operación falló";
            var statusCode = 400;

            // Act
            var result = ResultDto<string>.Failure(code, message, messageUser, statusCode);

            // Assert
            Assert.Null(result.Response); // Default value for reference type
            Assert.Equal(code, result.Code);
            Assert.Equal(message, result.Message);
            Assert.Equal(messageUser, result.MessageUser);
            Assert.Equal(statusCode, result.StatusCode);
        }

        [Fact]
        public void ResultDto_Failure_Return_Default_Value_Types()
        {
            // Arrange & Act
            var result = ResultDto<int>.Failure("ERROR", "Failed", "Falló", 400);

            // Assert
            Assert.Equal(0, result.Response); // Default value for int
        }

        [Fact]
        public void ResultDto_Success_Should_Handle_Complex_Objects()
        {
            // Arrange
            var complexObject = new { Id = 1, Name = "Test" };

            // Act
            var result = ResultDto<object>.Success(complexObject, "SUCCESS", "OK", "OK", 200);

            // Assert
            Assert.Equal(complexObject, result.Response);
            Assert.Equal("SUCCESS", result.Code);
        }
    }
}

