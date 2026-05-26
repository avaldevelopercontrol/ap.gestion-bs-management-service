using Moq;
using SubMgmt.Application.DTOs;
using SubMgmt.Application.DTOs.CancelSuscription;
using SubMgmt.Application.Interfaces;
using SubMgmt.Application.Validators;
using SubMgmt.Domain.Constants;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Validators_Test
{
    public class CancelSubscriptionRequestValidator_Tests
    {

        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IValidationMessageService> _mockValidationMessageService;
        private readonly Mock<IOriginRepository> _mockOriginRepository;
        private readonly Mock<ICommerceRepository> _mockCommerceRepository;
        private readonly Mock<ISubscriptionRepository> _mockSuscriptionRepository;
        private readonly Mock<IMovementRepository> _mockMovementRepository;

        public CancelSubscriptionRequestValidator_Tests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidationMessageService = new Mock<IValidationMessageService>();
            _mockOriginRepository = new Mock<IOriginRepository>();
            _mockCommerceRepository = new Mock<ICommerceRepository>();
            _mockSuscriptionRepository = new Mock<ISubscriptionRepository>();
            _mockMovementRepository = new Mock<IMovementRepository>();

            _mockUnitOfWork.Setup(x => x.Origins).Returns(_mockOriginRepository.Object);
            _mockUnitOfWork.Setup(x => x.Commerces).Returns(_mockCommerceRepository.Object);
            _mockUnitOfWork.Setup(x => x.Suscriptions).Returns(_mockSuscriptionRepository.Object);
            _mockUnitOfWork.Setup(x => x.Movements).Returns(_mockMovementRepository.Object);
        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_Language_Is_Invalid()
        {
            // Arrange
            var request = new CancelSubscriptionRequestDto
            {
                Language = "FR", // Idioma inválido
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS123",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CancelSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(ConstMsgVal.LANGUAGE_NOT_SUPPORTED, Const.LANGUAGE_ESP))
                .ReturnsAsync(new ValidationMessageDto { Code = "LANG_001", Message = "Language not supported" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.Equal("LANG_001", result.Code);
        }

      

        [Fact]
        public async Task Validate_Should_Create_Validator_Successfully()
        {
            // Arrange
            var request = new CancelSubscriptionRequestDto
            {
                Language = "ES",
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS123",
                User = "testuser",
                Origin = "API"
            };

            // Act
            var validator = new CancelSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            // Assert
            Assert.NotNull(validator);
            Assert.Equal(0, validator.MerchantId); // Valor por defecto
        }

        [Fact]
        public async Task Validate_Should_Call_ValidationMessageService()
        {
            // Arrange
            var request = new CancelSubscriptionRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS123",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CancelSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            await validator.Validate();

            // Assert
            _mockValidationMessageService.Verify(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void Validator_Should_Have_Required_Properties()
        {
            // Arrange
            var request = new CancelSubscriptionRequestDto
            {
                Language = "ES",
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS123",
                User = "testuser",
                Origin = "API"
            };

            // Act
            var validator = new CancelSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            // Assert
            Assert.NotNull(validator);
            Assert.True(validator.MerchantId >= 0); // Propiedad existe
        }

        [Fact]
        public async Task Validate_Should_Return_ResultDto_With_Correct_Type()
        {
            // Arrange
            var request = new CancelSubscriptionRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS123",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CancelSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResultDto<CancelSubscriptionResponseDto>>(result);
            Assert.NotNull(result.Code);
            Assert.NotNull(result.Message);
        }

        [Fact]
        public async Task Validate_Should_Handle_Empty_Strings()
        {
            // Arrange
            var request = new CancelSubscriptionRequestDto
            {
                Language = "",
                MerchantCode = "",
                SuscriptionId = "",
                User = "",
                Origin = ""
            };

            var validator = new CancelSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Const.SUCCESS_CODE, result.Code); // Debe fallar
        }

        [Fact]
        public async Task Validate_Should_Return_BadRequest_StatusCode_On_Error()
        {
            // Arrange
            var request = new CancelSubscriptionRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS123",
                User = "testuser",
                Origin = "API"
            };

            var validator = new CancelSubscriptionRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

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
