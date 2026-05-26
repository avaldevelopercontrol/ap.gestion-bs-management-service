
using Moq;
using SubMgmt.Application.Services;
using SubMgmt.Domain.Entities;
using SubMgmt.Domain.Interfaces;

namespace SubMgmt.UnitTests.SubMgmt_Application_Test.Services_Test
{
    public class ChargeTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChargeTypeRepository> _mockChargeTypeRepository;
        private readonly ChargeTypeService _service;

        public ChargeTypeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockChargeTypeRepository = new Mock<IChargeTypeRepository>();
            _mockUnitOfWork.Setup(x => x.ChargeTypes).Returns(_mockChargeTypeRepository.Object);
            _service = new ChargeTypeService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetChargeTypesAsync_Return_ChargeTypes_From_Repository()
        {
            // Arrange
            var expectedChargeTypes = new List<ChargeType>
            {
                new ChargeType { ChargeTypeId = 1, Name = "Fixed" },
                new ChargeType { ChargeTypeId = 2, Name = "Variable" }
            }.AsQueryable();

            _mockChargeTypeRepository
                .Setup(x => x.GetChargeTypesAsync())
                .ReturnsAsync(expectedChargeTypes);

            // Act
            var result = await _service.GetChargeTypesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedChargeTypes, result);
            _mockChargeTypeRepository.Verify(x => x.GetChargeTypesAsync(), Times.Once);
        }

        [Fact]
        public async Task ChargeTypesAsync_Call_Repository_Once()
        {
            // Arrange
            var expectedChargeTypes = new List<ChargeType>().AsQueryable();
            _mockChargeTypeRepository
                .Setup(x => x.GetChargeTypesAsync())
                .ReturnsAsync(expectedChargeTypes);

            // Act
            await _service.GetChargeTypesAsync();

            // Assert
            _mockChargeTypeRepository.Verify(x => x.GetChargeTypesAsync(), Times.Once);
        }

    }
}
