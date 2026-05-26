

using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class ChargeTypeTests
    {
        [Fact]
        public void ChargeType_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var chargeType = new ChargeType
            {
                ChargeTypeId = 1,
                Name = "Monthly"
            };

            // Assert
            Assert.Equal(1, chargeType.ChargeTypeId);
            Assert.Equal("Monthly", chargeType.Name);
        }

        [Fact]
        public void ChargeType_ShouldAllowSubscriptionsCollection()
        {
            // Arrange
            var chargeType = new ChargeType();
            var subscriptions = new List<Subscription>();

            // Act
            chargeType.suscriptions = subscriptions;

            // Assert
            Assert.NotNull(chargeType.suscriptions);
            Assert.Empty(chargeType.suscriptions);
        }

    }
}
