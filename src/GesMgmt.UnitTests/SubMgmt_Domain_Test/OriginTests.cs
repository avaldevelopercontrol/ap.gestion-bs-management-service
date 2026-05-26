
using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class OriginTests
    {
        [Fact]
        public void Origin_ShouldSetAndGetProperties()
        {
            // Arrange & Act
            var origin = new Origin
            {
                OriginId = 1,
                Name = "Web Portal"
            };

            // Assert
            Assert.Equal(1, origin.OriginId);
            Assert.Equal("Web Portal", origin.Name);
        }

        [Fact]
        public void Origin_ShouldAllowSubscriptionsCollection()
        {
            // Arrange
            var origin = new Origin();
            var subscriptions = new List<Subscription>();

            // Act
            origin.suscriptions = subscriptions;

            // Assert
            Assert.NotNull(origin.suscriptions);
            Assert.Empty(origin.suscriptions);
        }

        [Fact]
        public void Origin_ShouldAllowEmptyValues()
        {
            // Arrange & Act
            var origin = new Origin();

            // Assert
            Assert.Equal(0, origin.OriginId);
            Assert.Null(origin.Name);
            Assert.Null(origin.suscriptions);
        }

        [Fact]
        public void Origin_ShouldHandleDifferentOriginTypes()
        {
            // Arrange & Act
            var mobileOrigin = new Origin { OriginId = 1, Name = "Mobile App" };
            var webOrigin = new Origin { OriginId = 2, Name = "Web Portal" };
            var apiOrigin = new Origin { OriginId = 3, Name = "API Integration" };

            // Assert
            Assert.Equal("Mobile App", mobileOrigin.Name);
            Assert.Equal("Web Portal", webOrigin.Name);
            Assert.Equal("API Integration", apiOrigin.Name);
        }
    }
}
