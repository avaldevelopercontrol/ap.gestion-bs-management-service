using Microsoft.Extensions.Caching.Memory;
using Moq;
using SubMgmt.Application.Services;
using SubMgmt.Domain.Constants;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Services_Test
{
    public class ValidationMessageServiceTests
    {
        private readonly IMemoryCache _cache; // Usar implementación real
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IValidationMessageRepository> _mockValidationMessageRepository;
        private readonly ValidationMessageService _service;

        public ValidationMessageServiceTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions()); // Implementación real
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidationMessageRepository = new Mock<IValidationMessageRepository>();
            _mockUnitOfWork.Setup(x => x.ValidationMessages).Returns(_mockValidationMessageRepository.Object);
            _service = new ValidationMessageService(_cache, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetByCode_Should_Return_Spanish_Message_When_Language_Is_ESP()
        {
            // Arrange
            var code = "TEST001";
            var language = Const.LANGUAGE_ESP;
            var validationMessages = new List<ValidationMessage>
            {
                new ValidationMessage
                {
                    Code = code,
                    Message_ESP = "Mensaje en español",
                    Message_Friendy_ESP = "Mensaje amigable en español",
                    Message_ENG = "English message",
                    Message_Friendy_ENG = "Friendly English message"
                }
            };

            // Precargar el cache
            _cache.Set(Const.MESSAGES_CACHE_KEY, validationMessages);

            // Act
            var result = await _service.GetByCode(code, language);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(code, result.Code);
            Assert.Equal("Mensaje en español", result.Message);
            Assert.Equal("Mensaje amigable en español", result.MessageFriendly);
        }

        [Fact]
        public async Task GetByCode_Should_Return_English_Message_When_Language_Is_Not_ESP()
        {
            // Arrange
            var code = "TEST001";
            var language = "ENG";
            var validationMessages = new List<ValidationMessage>
            {
                new ValidationMessage
                {
                    Code = code,
                    Message_ESP = "Mensaje en español",
                    Message_Friendy_ESP = "Mensaje amigable en español",
                    Message_ENG = "English message",
                    Message_Friendy_ENG = "Friendly English message"
                }
            };

            // Precargar el cache
            _cache.Set(Const.MESSAGES_CACHE_KEY, validationMessages);

            // Act
            var result = await _service.GetByCode(code, language);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(code, result.Code);
            Assert.Equal("English message", result.Message);
            Assert.Equal("Friendly English message", result.MessageFriendly);
        }

        [Fact]
        public async Task GetByCode_Should_Return_NotFound_Message_When_Code_Does_Not_Exist()
        {
            // Arrange
            var code = "NONEXISTENT";
            var language = Const.LANGUAGE_ESP;
            var validationMessages = new List<ValidationMessage>
            {
                new ValidationMessage
                {
                    Code = ConstMsgVal.MESSAGE_CODE_NOT_FOUND,
                    Message_ESP = "Código no encontrado",
                    Message_Friendy_ESP = "El código solicitado no existe",
                    Message_ENG = "Code not found",
                    Message_Friendy_ENG = "The requested code does not exist"
                }
            };

            // Precargar el cache
            _cache.Set(Const.MESSAGES_CACHE_KEY, validationMessages);

            // Act
            var result = await _service.GetByCode(code, language);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ConstMsgVal.MESSAGE_CODE_NOT_FOUND, result.Code);
            Assert.Equal("Código no encontrado", result.Message);
            Assert.Equal("El código solicitado no existe", result.MessageFriendly);
        }

        [Fact]
        public async Task GetByCode_Should_Fetch_From_Repository_When_Not_In_Cache()
        {
            // Arrange
            var code = "TEST001";
            var validationMessages = new List<ValidationMessage>
            {
                new ValidationMessage
                {
                    Code = code,
                    Message_ESP = "Mensaje en español",
                    Message_Friendy_ESP = "Mensaje amigable en español",
                    Message_ENG = "English message",
                    Message_Friendy_ENG = "Friendly English message"
                }
            };

            _mockValidationMessageRepository
                .Setup(x => x.GetMessages())
                .ReturnsAsync(validationMessages);

            // Act
            var result = await _service.GetByCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(code, result.Code);
            _mockValidationMessageRepository.Verify(x => x.GetMessages(), Times.Once);

            // Verificar que se guardó en cache
            var cachedResult = _cache.Get<IEnumerable<ValidationMessage>>(Const.MESSAGES_CACHE_KEY);
            Assert.NotNull(cachedResult);
            Assert.Single(cachedResult);
        }

        [Fact]
        public void RefreshValidationMessages_Should_Remove_Cache_Entry()
        {
            // Arrange
            var validationMessages = new List<ValidationMessage>
            {
                new ValidationMessage { Code = "TEST", Message_ESP = "Test" }
            };
            _cache.Set(Const.MESSAGES_CACHE_KEY, validationMessages);

            // Verificar que está en cache
            var cachedBefore = _cache.Get(Const.MESSAGES_CACHE_KEY);
            Assert.NotNull(cachedBefore);

            // Act
            _service.RefreshValidationMessages();

            // Assert
            var cachedAfter = _cache.Get(Const.MESSAGES_CACHE_KEY);
            Assert.Null(cachedAfter);
        }

        [Fact]
        public async Task GetByCode_Should_Handle_Null_Language_Parameter()
        {
            // Arrange
            var code = "TEST001";
            var validationMessages = new List<ValidationMessage>
            {
                new ValidationMessage
                {
                    Code = code,
                    Message_ESP = "Mensaje en español",
                    Message_Friendy_ESP = "Mensaje amigable en español"
                }
            };

            _cache.Set(Const.MESSAGES_CACHE_KEY, validationMessages);

            // Act
            var result = await _service.GetByCode(code); // Sin parámetro language

            // Assert
            Assert.NotNull(result);
            Assert.Equal(code, result.Code);
            Assert.Equal("Mensaje en español", result.Message); // Debería usar ESP por defecto
        }

        public void Dispose()
        {
            _cache?.Dispose();
        }
    }
}
