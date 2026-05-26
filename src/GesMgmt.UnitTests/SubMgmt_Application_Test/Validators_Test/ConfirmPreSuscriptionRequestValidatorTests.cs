using Moq;
using SubMgmt.Application.DTOs;
using SubMgmt.Application.DTOs.ConfirmPreSubscription;
using SubMgmt.Application.Interfaces;
using SubMgmt.Application.Validators;
using SubMgmt.Domain.Constants;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Validators_Test
{
    public class ConfirmPreSuscriptionRequestValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IValidationMessageService> _mockValidationMessageService;
        private readonly Mock<IOriginRepository> _mockOriginRepository;
        private readonly Mock<ICommerceRepository> _mockCommerceRepository;
        private readonly Mock<IPreSubscriptionRepository> _mockPreSuscriptionRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;

        public ConfirmPreSuscriptionRequestValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidationMessageService = new Mock<IValidationMessageService>();
            _mockOriginRepository = new Mock<IOriginRepository>();
            _mockCommerceRepository = new Mock<ICommerceRepository>();
            _mockPreSuscriptionRepository = new Mock<IPreSubscriptionRepository>();
            _mockProductRepository = new Mock<IProductRepository>();

            _mockUnitOfWork.Setup(x => x.Origins).Returns(_mockOriginRepository.Object);
            _mockUnitOfWork.Setup(x => x.Commerces).Returns(_mockCommerceRepository.Object);
            _mockUnitOfWork.Setup(x => x.PreSuscriptions).Returns(_mockPreSuscriptionRepository.Object);
            _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_Language_Length_Is_Invalid()
        {
            // Arrange
            var request = new ConfirmPreSubscriptionRequestDto
            {
                Language = "", // Idioma vacío
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API",
            };

            var validator = new ConfirmPreSuscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(ConstMsgVal.LANGUAGE_LENGTH_INVALID, Const.LANGUAGE_ESP))
                .ReturnsAsync(new ValidationMessageDto { Code = "LANG_001", Message = "Language length invalid" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.Equal("LANG_001", result.Code);
        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_Language_Is_Invalid()
        {
            // Arrange
            var request = new ConfirmPreSubscriptionRequestDto
            {
                Language = "FR", // Idioma inválido
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API",
            };

            var validator = new ConfirmPreSuscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(ConstMsgVal.LANGUAGE_NOT_SUPPORTED, Const.LANGUAGE_ESP))
                .ReturnsAsync(new ValidationMessageDto { Code = "LANG_002", Message = "Language not supported" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.Equal("LANG_002", result.Code);
        }

        [Fact]
        public void Validator_Should_Create_Successfully()
        {
            // Arrange
            var request = new ConfirmPreSubscriptionRequestDto
            {
                Language = "ES",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API",
            };

            // Act
            var validator = new ConfirmPreSuscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            // Assert
            Assert.NotNull(validator);
            Assert.Equal(0, validator.OriginId); // Valor por defecto
            Assert.Equal(0, validator.MerchantId); // Valor por defecto
        }

        [Fact]
        public async Task Validate_Should_Return_ResultDto_With_Correct_Type()
        {
            // Arrange
            var request = new ConfirmPreSubscriptionRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API",
            };

            var validator = new ConfirmPreSuscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResultDto<ConfirmPreSubscriptionResponseDto>>(result);
            Assert.NotNull(result.Code);
        }

        [Fact]
        public async Task Validate_Should_Call_ValidationMessageService()
        {
            // Arrange
            var request = new ConfirmPreSubscriptionRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API",
            };

            var validator = new ConfirmPreSuscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            await validator.Validate();

            // Assert
            _mockValidationMessageService.Verify(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Validate_Should_Handle_Empty_Strings()
        {
            // Arrange
            var request = new ConfirmPreSubscriptionRequestDto
            {
                Language = "",
                MerchantCode = "",
                User = "testuser",
                Origin = "API",
            };

            var validator = new ConfirmPreSuscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Const.SUCCESS_CODE, result.Code);
        }

       

        [Fact]
        public void Validator_Should_Have_Required_Properties()
        {
            // Arrange
            var request = new ConfirmPreSubscriptionRequestDto
            {
                Language = "ES",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API",
            };

            // Act
            var validator = new ConfirmPreSuscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            // Assert
            Assert.True(validator.OriginId >= 0);
            Assert.True(validator.MerchantId >= 0);
            Assert.Null(validator.ContractStartDt);
            Assert.Null(validator.Amount);
        }

    }
}
