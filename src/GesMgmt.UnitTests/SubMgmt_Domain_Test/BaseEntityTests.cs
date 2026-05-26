using SubMgmt.Domain.Entities;

namespace SubMgmt.UnitTests.SubMgmt_Domain_Test
{
    public class BaseEntityTests
    {
        private class TestEntity : BaseEntity
        {
            public int Id { get; set; }
        }

        [Fact]
        public void BaseEntity_ShouldHaveUserCreatedProperty()
        {
            // Arrange & Act
            var entity = new TestEntity();
            entity.UserCreated = "testuser";

            // Assert
            Assert.Equal("testuser", entity.UserCreated);
        }

        [Fact]
        public void BaseEntity_ShouldHaveCreatedAtProperty()
        {
            // Arrange & Act
            var entity = new TestEntity();
            var date = DateTime.Now;
            entity.CreatedAt = date;

            // Assert
            Assert.Equal(date, entity.CreatedAt);
        }

        [Fact]
        public void BaseEntity_ShouldHaveUserUpdatedProperty()
        {
            // Arrange & Act
            var entity = new TestEntity();
            entity.UserUpdated = "updateuser";

            // Assert
            Assert.Equal("updateuser", entity.UserUpdated);
        }

        [Fact]
        public void BaseEntity_ShouldHaveUpdatedAtProperty()
        {
            // Arrange & Act
            var entity = new TestEntity();
            var date = DateTime.Now;
            entity.UpdatedAt = date;

            // Assert
            Assert.Equal(date, entity.UpdatedAt);
        }

        [Fact]
        public void BaseEntity_PropertiesCanBeNull()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            Assert.Null(entity.UserCreated);
            Assert.Null(entity.CreatedAt);
            Assert.Null(entity.UserUpdated);
            Assert.Null(entity.UpdatedAt);
        }

    }
}
