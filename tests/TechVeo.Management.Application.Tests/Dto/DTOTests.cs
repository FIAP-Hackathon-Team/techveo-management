using TechVeo.Management.Application.Dto;
using TechVeo.Management.Domain.Enums;

namespace TechVeo.Management.Application.Tests.Dto;

public class VideoDtoTests
{
    [Fact(DisplayName = "VideoDto can be created with all properties")]
    [Trait("Application", "VideoDto")]
    public void ShouldCreateVideoDto_WithAllProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = Status.Completed;
        var fileName = "test-video.mp4";
        var intervalSeconds = 10.5;
        var snapshotCount = 5;
        var width = 1920;
        var height = 1080;

        // Act
        var dto = new VideoDto(id, status, fileName, intervalSeconds, snapshotCount, width, height);

        // Assert
        dto.Id.Should().Be(id);
        dto.Status.Should().Be(status);
        dto.FileName.Should().Be(fileName);
        dto.IntervalSeconds.Should().Be(intervalSeconds);
        dto.SnapshotCount.Should().Be(snapshotCount);
        dto.Width.Should().Be(width);
        dto.Height.Should().Be(height);
    }

    [Fact(DisplayName = "VideoDto can be created without optional parameters")]
    [Trait("Application", "VideoDto")]
    public void ShouldCreateVideoDto_WithoutOptionalParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = Status.Queued;

        // Act
        var dto = new VideoDto(id, status, null, null, null, 1920, 1080);

        // Assert
        dto.Id.Should().Be(id);
        dto.Status.Should().Be(status);
        dto.FileName.Should().BeNull();
        dto.IntervalSeconds.Should().BeNull();
        dto.SnapshotCount.Should().BeNull();
    }

    [Fact(DisplayName = "VideoDto properties are immutable")]
    [Trait("Application", "VideoDto")]
    public void VideoDtoPropertiesAreImmutable()
    {
        // Arrange
        var dto1 = new VideoDto(Guid.NewGuid(), Status.Queued, "video1.mp4", null, null, 1920, 1080);
        var dto2 = new VideoDto(Guid.NewGuid(), Status.Failed, "video2.mp4", 5.0, 10, 1280, 720);

        // Assert
        dto1.FileName.Should().Be("video1.mp4");
        dto2.FileName.Should().Be("video2.mp4");
        dto1.Should().NotBe(dto2);
    }

    [Fact(DisplayName = "VideoDto with different statuses")]
    [Trait("Application", "VideoDto")]
    public void VideoDtoWithDifferentStatuses()
    {
        // Arrange & Act
        var queuedDto = new VideoDto(Guid.NewGuid(), Status.Queued, "test.mp4", null, null, 1920, 1080);
        var processingDto = new VideoDto(Guid.NewGuid(), Status.Processing, "test.mp4", null, null, 1920, 1080);
        var completedDto = new VideoDto(Guid.NewGuid(), Status.Completed, "test.mp4", 10.0, 5, 1920, 1080);
        var failedDto = new VideoDto(Guid.NewGuid(), Status.Failed, "test.mp4", null, null, 1920, 1080);

        // Assert
        queuedDto.Status.Should().Be(Status.Queued);
        processingDto.Status.Should().Be(Status.Processing);
        completedDto.Status.Should().Be(Status.Completed);
        failedDto.Status.Should().Be(Status.Failed);
    }
}

public class UserDtoTests
{
    [Fact(DisplayName = "UserDto can be created with all properties")]
    [Trait("Application", "UserDto")]
    public void ShouldCreateUserDto_WithAllProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var username = "johndoe";
        var email = "john@example.com";
        var role = "Admin";

        // Act
        var dto = new UserDto(id, name, username, email, role);

        // Assert
        dto.Id.Should().Be(id);
        dto.Name.Should().Be(name);
        dto.Username.Should().Be(username);
        dto.Email.Should().Be(email);
        dto.Role.Should().Be(role);
    }

    [Fact(DisplayName = "UserDto can be created without email")]
    [Trait("Application", "UserDto")]
    public void ShouldCreateUserDto_WithoutEmail()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var dto = new UserDto(id, "Jane Doe", "janedoe", null, "User");

        // Assert
        dto.Email.Should().BeNull();
        dto.Id.Should().Be(id);
    }

    [Fact(DisplayName = "Different UserDtos are not equal")]
    [Trait("Application", "UserDto")]
    public void DifferentUserDtos_ShouldNotBeEqual()
    {
        // Arrange
        var dto1 = new UserDto(Guid.NewGuid(), "User1", "user1", "user1@example.com", "User");
        var dto2 = new UserDto(Guid.NewGuid(), "User2", "user2", "user2@example.com", "Admin");

        // Assert
        dto1.Should().NotBe(dto2);
    }
}
