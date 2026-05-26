
using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class SubscriptionStatusTests
    {
        [Fact]
        public void SubscriptionStatus_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var subscriptionStatus = new SubscriptionStatus
            {
                SubStatId = 1,
                Name = "Active"
            };

            // Assert
            Assert.Equal(1, subscriptionStatus.SubStatId);
            Assert.Equal("Active", subscriptionStatus.Name);
        }

        [Fact]
        public void SubscriptionStatus_ShouldAllowSubscriptionsCollection()
        {
            // Arrange
            var subscriptionStatus = new SubscriptionStatus();
            var subscriptions = new List<Subscription>();

            // Act
            subscriptionStatus.suscriptions = subscriptions;

            // Assert
            Assert.NotNull(subscriptionStatus.suscriptions);
            Assert.Empty(subscriptionStatus.suscriptions);
        }

        [Fact]
        public void SubscriptionStatus_ShouldHandleDifferentStatuses()
        {
            // Arrange & Act
            var activeStatus = new SubscriptionStatus { SubStatId = 1, Name = "Active" };
            var pausedStatus = new SubscriptionStatus { SubStatId = 2, Name = "Paused" };
            var cancelledStatus = new SubscriptionStatus { SubStatId = 3, Name = "Cancelled" };
            var expiredStatus = new SubscriptionStatus { SubStatId = 4, Name = "Expired" };

            // Assert
            Assert.Equal("Active", activeStatus.Name);
            Assert.Equal("Paused", pausedStatus.Name);
            Assert.Equal("Cancelled", cancelledStatus.Name);
            Assert.Equal("Expired", expiredStatus.Name);
        }

        [Fact]
        public void SubscriptionStatus_ShouldAllowEmptyValues()
        {
            // Arrange & Act
            var subscriptionStatus = new SubscriptionStatus();

            // Assert
            Assert.Equal(0, subscriptionStatus.SubStatId);
            Assert.Null(subscriptionStatus.Name);
            Assert.Null(subscriptionStatus.suscriptions);
        }

    }
}
