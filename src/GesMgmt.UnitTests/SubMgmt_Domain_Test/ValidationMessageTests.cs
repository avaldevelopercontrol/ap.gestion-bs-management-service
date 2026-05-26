using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class ValidationMessageTests
    {
        [Fact]
        public void ValidationMessage_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var validationMessage = new ValidationMessage
            {
                Id = 1,
                Code = "ERR001",
                Message_ESP = "Error en español",
                Message_ENG = "Error in english",
                Message_Friendy_ESP = "Error amigable español",
                Message_Friendy_ENG = "Friendly error english",
                Action = "RETRY",
                Api = "PaymentAPI"
            };

            // Assert
            Assert.Equal(1, validationMessage.Id);
            Assert.Equal("ERR001", validationMessage.Code);
            Assert.Equal("Error en español", validationMessage.Message_ESP);
            Assert.Equal("Error in english", validationMessage.Message_ENG);
            Assert.Equal("Error amigable español", validationMessage.Message_Friendy_ESP);
            Assert.Equal("Friendly error english", validationMessage.Message_Friendy_ENG);
            Assert.Equal("RETRY", validationMessage.Action);
            Assert.Equal("PaymentAPI", validationMessage.Api);
        }

        [Fact]
        public void ValidationMessage_ShouldAllowEmptyValues()
        {
            // Arrange & Act
            var validationMessage = new ValidationMessage();

            // Assert
            Assert.Equal(0, validationMessage.Id);
            Assert.Null(validationMessage.Code);
            Assert.Null(validationMessage.Message_ESP);
            Assert.Null(validationMessage.Message_ENG);
        }

    }
}
