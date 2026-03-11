using MediatR;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Application.Events.Integration.Incoming;
using TechVeo.Management.Application.Events.Integration.Incoming.Handlers;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Application.Services.Interfaces;
using TechVeo.Management.Domain.Entities;
using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Tests.Events.Integration.Incoming.Handlers;

public class VideoProcessingFailedHandlerTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly VideoProcessingFailedHandler _handler;

    public VideoProcessingFailedHandlerTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new VideoProcessingFailedHandler(
            _mediatorMock.Object,
            _videoRepositoryMock.Object,
            _authServiceMock.Object);
    }

    [Fact(DisplayName = "Should set video status to Failed")]
    [Trait("Application", "VideoProcessingFailedHandler")]
    public async Task Handle_WithValidEvent_ShouldSetStatusToFailed()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var video = new Video(userId, "failed-video.mp4", null, null, 1920, 1080);
        var failedAt = DateTime.UtcNow;

        var user = new UserDto(userId, "John Doe", "johndoe", "john@example.com", "User");

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(videoId))
            .ReturnsAsync(video);

        _authServiceMock
            .Setup(x => x.GetUserBydIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var evt = new VideoProcessingFailedEvent(videoId, failedAt);

        // Act
        await _handler.Handle(evt, CancellationToken.None);

        // Assert
        video.Status.Should().Be(Status.Failed);
    }

    [Fact(DisplayName = "Should send email notification with correct details")]
    [Trait("Application", "VideoProcessingFailedHandler")]
    public async Task Handle_WithValidEvent_ShouldPublishEmailEvent()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var fileName = "test-failed-video.mp4";
        
        var video = new Video(userId, fileName, null, null, 1920, 1080);
        var user = new UserDto(userId, "John Doe", "johndoe", email, "User");

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(videoId))
            .ReturnsAsync(video);

        _authServiceMock
            .Setup(x => x.GetUserBydIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var evt = new VideoProcessingFailedEvent(videoId, DateTime.UtcNow);

        // Act
        await _handler.Handle(evt, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(
            x => x.Publish(
                It.Is<SendEmailEvent>(e =>
                    e.EmailAddress == email &&
                    e.FileName == fileName &&
                    e.Status == Status.Failed &&
                    e.Url == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should not send email when video not found")]
    [Trait("Application", "VideoProcessingFailedHandler")]
    public async Task Handle_WithNonExistentVideo_ShouldNotProcessEvent()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var failedAt = DateTime.UtcNow;

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(videoId))
            .ReturnsAsync((Video?)null);

        var evt = new VideoProcessingFailedEvent(videoId, failedAt);

        // Act
        await _handler.Handle(evt, CancellationToken.None);

        // Assert
        _authServiceMock.Verify(
            x => x.GetUserBydIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mediatorMock.Verify(
            x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Should handle multiple failed videos independently")]
    [Trait("Application", "VideoProcessingFailedHandler")]
    public async Task Handle_WithMultipleFailedVideos_ShouldProcessEachIndependently()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var video1 = new Video(userId1, "video1.mp4", null, null, 1920, 1080);
        var video2 = new Video(userId2, "video2.mp4", null, null, 1280, 720);
        
        var user1 = new UserDto(userId1, "User One", "user1", "user1@example.com", "User");
        var user2 = new UserDto(userId2, "User Two", "user2", "user2@example.com", "User");

        _videoRepositoryMock
            .SetupSequence(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(video1)
            .ReturnsAsync(video2);

        _authServiceMock
            .SetupSequence(x => x.GetUserBydIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1)
            .ReturnsAsync(user2);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var evt1 = new VideoProcessingFailedEvent(Guid.NewGuid(), DateTime.UtcNow);
        var evt2 = new VideoProcessingFailedEvent(Guid.NewGuid(), DateTime.UtcNow);

        // Act
        await _handler.Handle(evt1, CancellationToken.None);
        await _handler.Handle(evt2, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(
            x => x.Publish(It.IsAny<SendEmailEvent>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}
