
using Moq;
using SubMgmt.Application.DTOs;
using SubMgmt.Application.DTOs.CreateSubscription;
using SubMgmt.Application.Interfaces;
using SubMgmt.Application.Validators;
using SubMgmt.Domain.Constants;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Validators_Test
{
    public class CreateSubscriptionRequestValidatorTests
    {

        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IValidationMessageService> _mockValidationMessageService;
        private readonly Mock<IOriginRepository> _mockOriginRepository;
        private readonly Mock<ICommerceRepository> _mockCommerceRepository;
        private readonly Mock<ICardBrandRepository> _mockCardBrandRepository;
        private readonly Mock<IBuyerRepository> _mockBuyerRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;

        public CreateSubscriptionRequestValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidationMessageService = new Mock<IValidationMessageService>();
            _mockOriginRepository = new Mock<IOriginRepository>();
            _mockCommerceRepository = new Mock<ICommerceRepository>();
            _mockCardBrandRepository = new Mock<ICardBrandRepository>();
            _mockBuyerRepository = new Mock<IBuyerRepository>();
            _mockProductRepository = new Mock<IProductRepository>();

            _mockUnitOfWork.Setup(x => x.Origins).Returns(_mockOriginRepository.Object);
            _mockUnitOfWork.Setup(x => x.Commerces).Returns(_mockCommerceRepository.Object);
            _mockUnitOfWork.Setup(x => x.CardBrands).Returns(_mockCardBrandRepository.Object);
            _mockUnitOfWork.Setup(x => x.Buyers).Returns(_mockBuyerRepository.Object);
            _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_Language_Length_Is_Invalid()
        {
            // Arrange
            var request = new CreateSubscriptionRequestDto
            {
                Language = "", // Idioma vacío
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CreateSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

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
            var request = new CreateSubscriptionRequestDto
            {
                Language = "FR", // Idioma inválido
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CreateSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

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
            var request = new CreateSubscriptionRequestDto
            {
                Language = "ES",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API"
            };

            // Act
            var validator = new CreateSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            // Assert
            Assert.NotNull(validator);
            Assert.Equal(0, validator.MerchantId);
            Assert.Equal(0, validator.ProductId);
            Assert.Equal(0, validator.BuyerId);
            Assert.Empty(validator.MerchantBuyerId ?? "");
            Assert.Empty(validator.CardToken ?? "");
        }

        [Fact]
        public async Task Validate_Should_Return_ResultDto_With_Correct_Type()
        {
            // Arrange
            var request = new CreateSubscriptionRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CreateSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResultDto<CreateSubscriptionResponseDto>>(result);
            Assert.NotNull(result.Code);
        }

        [Fact]
        public async Task Validate_Should_Call_ValidationMessageService()
        {
            // Arrange
            var request = new CreateSubscriptionRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CreateSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

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
            var request = new CreateSubscriptionRequestDto
            {
                Language = "",
                MerchantCode = "",
                User = "",
                Origin = ""
            };

            var validator = new CreateSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

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
            var request = new CreateSubscriptionRequestDto
            {
                Language = "ES",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API"
            };

            // Act
            var validator = new CreateSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            // Assert
            Assert.True(validator.MerchantId >= 0);
            Assert.True(validator.ProductId >= 0);
            Assert.True(validator.OriginId >= 0);
            Assert.Null(validator.Amount);
            Assert.Null(validator.ContractStartDt);
        }

        [Fact]
        public async Task Validate_Should_Return_BadRequest_StatusCode_On_Error()
        {
            // Arrange
            var request = new CreateSubscriptionRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CreateSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.Equal(Const.BAD_REQUEST_CODE, result.StatusCode);
        }

    }
}
