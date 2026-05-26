using Moq;
using SubMgmt.Application.DTOs;
using SubMgmt.Application.DTOs.SuscriptionDetails;
using SubMgmt.Application.Interfaces;
using SubMgmt.Application.Validators;
using SubMgmt.Domain.Constants;
using SubMgmt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Validators_Test
{
    public class GetSubscriptionDetailRequestValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IValidationMessageService> _mockValidationMessageService;
        private readonly Mock<ICommerceRepository> _mockCommerceRepository;
        private readonly Mock<IOriginRepository> _mockOriginRepository;
        private readonly Mock<ISubscriptionStatusRepository> _mockSuscriptionStatusRepository;

        public GetSubscriptionDetailRequestValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidationMessageService = new Mock<IValidationMessageService>();
            _mockCommerceRepository = new Mock<ICommerceRepository>();
            _mockOriginRepository = new Mock<IOriginRepository>();
            _mockSuscriptionStatusRepository = new Mock<ISubscriptionStatusRepository>();

            _mockUnitOfWork.Setup(x => x.Commerces).Returns(_mockCommerceRepository.Object);
            _mockUnitOfWork.Setup(x => x.Origins).Returns(_mockOriginRepository.Object);
            _mockUnitOfWork.Setup(x => x.SuscriptionStatus).Returns(_mockSuscriptionStatusRepository.Object);
        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_Language_Length_Is_Invalid()
        {
            // Arrange
            var request = new GetSuscriptionDetailRequestDto
            {
                Language = "", // Idioma vacío
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS001"
            };

            var validator = new GetSubscriptionDetailRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

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
            var request = new GetSuscriptionDetailRequestDto
            {
                Language = "FR", // Idioma inválido
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS002"
            };

            var validator = new GetSubscriptionDetailRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

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
            var request = new GetSuscriptionDetailRequestDto
            {
                Language = "ES",
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS003"
            };

            // Act
            var validator = new GetSubscriptionDetailRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            // Assert
            Assert.NotNull(validator);
            Assert.Equal(0, validator.MerchantId);
        }

        [Fact]
        public async Task Validate_Should_Return_ResultListDto_With_Correct_Type()
        {
            // Arrange
            var request = new GetSuscriptionDetailRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS003"
            };

            var validator = new GetSubscriptionDetailRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResultListDto<IEnumerable<GetSubscriptionResponseDto>>>(result);
            Assert.NotNull(result.Code);
        }

        [Fact]
        public async Task Validate_Should_Call_ValidationMessageService()
        {
            // Arrange
            var request = new GetSuscriptionDetailRequestDto
            {
                Language = "INVALID",
                MerchantCode = "MERCHANT001",
                SuscriptionId = "SUBS003"
            };

            var validator = new GetSubscriptionDetailRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

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
            var request = new GetSuscriptionDetailRequestDto
            {
                Language = "",
                MerchantCode = "",
                SuscriptionId = ""
            };

            var validator = new GetSubscriptionDetailRequestValidator(_mockUnitOfWork.Object, _mockValidationMessageService.Object, request);

            _mockValidationMessageService
                .Setup(x => x.GetByCode(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidationMessageDto { Code = "ERROR_001", Message = "Error" });

            // Act
            var result = await validator.Validate();

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Const.SUCCESS_CODE, result.Code);
        }

    }
}