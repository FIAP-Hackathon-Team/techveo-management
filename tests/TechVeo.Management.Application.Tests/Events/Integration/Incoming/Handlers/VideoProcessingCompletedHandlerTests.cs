using MediatR;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Application.Events.Integration.Incoming;
using TechVeo.Management.Application.Events.Integration.Incoming.Handlers;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Application.Services.Interfaces;
using TechVeo.Management.Domain.Entities;
using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Shared.Application.Storage;

namespace TechVeo.Management.Application.Tests.Events.Integration.Incoming.Handlers;

public class VideoProcessingCompletedHandlerTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IVideoStorage> _videoStorageMock;
    private readonly VideoProcessingCompletedHandler _handler;

    public VideoProcessingCompletedHandlerTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _mediatorMock = new Mock<IMediator>();
        _videoStorageMock = new Mock<IVideoStorage>();

        _handler = new VideoProcessingCompletedHandler(
            _mediatorMock.Object,
            _videoRepositoryMock.Object,
            _videoStorageMock.Object,
            _authServiceMock.Object);
    }

    [Fact(DisplayName = "Should set video status to Completed")]
    [Trait("Application", "VideoProcessingCompletedHandler")]
    public async Task Handle_WithValidEvent_ShouldSetStatusToCompleted()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var zipKey = "videos/completed/123.zip";
        var video = new Video(userId, "completed-video.mp4", null, null, 1920, 1080);
        video.SetFileKey("videos/123.mp4");

        var user = new UserDto(userId, "John Doe", "johndoe", "john@example.com", "User");
        var downloadUrl = "https://storage.example.com/videos/123.mp4?expires=...";

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(videoId))
            .ReturnsAsync(video);

        _authServiceMock
            .Setup(x => x.GetUserBydIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _videoStorageMock
            .Setup(x => x.GetVideoDownloadUrlAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(downloadUrl);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var evt = new VideoProcessingCompletedEvent(videoId, DateTime.UtcNow, zipKey);

        // Act
        await _handler.Handle(evt, CancellationToken.None);

        // Assert
        video.Status.Should().Be(Status.Completed);
    }

    [Fact(DisplayName = "Should generate download URL with correct expiration")]
    [Trait("Application", "VideoProcessingCompletedHandler")]
    public async Task Handle_WithValidEvent_ShouldGenerateDownloadUrl()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var zipKey = "videos/completed/123.zip";
        var fileKey = "videos/123.mp4";
        var downloadUrl = "https://storage.example.com/download?expires=...";
        
        var video = new Video(userId, "completed-video.mp4", null, null, 1920, 1080);
        video.SetFileKey(fileKey);

        var user = new UserDto(userId, "John Doe", "johndoe", "john@example.com", "User");

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(videoId))
            .ReturnsAsync(video);

        _authServiceMock
            .Setup(x => x.GetUserBydIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _videoStorageMock
            .Setup(x => x.GetVideoDownloadUrlAsync(fileKey, TimeSpan.FromHours(24), It.IsAny<CancellationToken>()))
            .ReturnsAsync(downloadUrl);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var evt = new VideoProcessingCompletedEvent(videoId, DateTime.UtcNow, zipKey);

        // Act
        await _handler.Handle(evt, CancellationToken.None);

        // Assert
        _videoStorageMock.Verify(
            x => x.GetVideoDownloadUrlAsync(fileKey, TimeSpan.FromHours(24), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should send email notification with download URL")]
    [Trait("Application", "VideoProcessingCompletedHandler")]
    public async Task Handle_WithValidEvent_ShouldPublishEmailEventWithUrl()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var fileName = "completed-video.mp4";
        var downloadUrl = "https://storage.example.com/download";
        var zipKey = "videos/completed/123.zip";
        
        var video = new Video(userId, fileName, null, null, 1920, 1080);
        video.SetFileKey("videos/123.mp4");

        var user = new UserDto(userId, "John Doe", "johndoe", email, "User");

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(videoId))
            .ReturnsAsync(video);

        _authServiceMock
            .Setup(x => x.GetUserBydIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _videoStorageMock
            .Setup(x => x.GetVideoDownloadUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(downloadUrl);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var evt = new VideoProcessingCompletedEvent(videoId, DateTime.UtcNow, zipKey);

        // Act
        await _handler.Handle(evt, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(
            x => x.Publish(
                It.Is<SendEmailEvent>(e =>
                    e.EmailAddress == email &&
                    e.FileName == fileName &&
                    e.Status == Status.Completed &&
                    e.Url == downloadUrl),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should not process when video not found")]
    [Trait("Application", "VideoProcessingCompletedHandler")]
    public async Task Handle_WithNonExistentVideo_ShouldNotProcessEvent()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var zipKey = "videos/completed/123.zip";

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(videoId))
            .ReturnsAsync((Video?)null);

        var evt = new VideoProcessingCompletedEvent(videoId, DateTime.UtcNow, zipKey);

        // Act
        await _handler.Handle(evt, CancellationToken.None);

        // Assert
        _authServiceMock.Verify(
            x => x.GetUserBydIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _videoStorageMock.Verify(
            x => x.GetVideoDownloadUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mediatorMock.Verify(
            x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Should handle completion with different video resolutions")]
    [Trait("Application", "VideoProcessingCompletedHandler")]
    public async Task Handle_With4KVideo_ShouldProcessSuccessfully()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var zipKey = "videos/completed/4k-123.zip";
        var downloadUrl = "https://storage.example.com/4k-download";
        
        var video = new Video(userId, "4k-video.mp4", 20, 5.0, 3840, 2160);
        video.SetFileKey("videos/4k-123.mp4");

        var user = new UserDto(userId, "John Doe", "johndoe", "john@example.com", "User");

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(videoId))
            .ReturnsAsync(video);

        _authServiceMock
            .Setup(x => x.GetUserBydIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _videoStorageMock
            .Setup(x => x.GetVideoDownloadUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(downloadUrl);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var evt = new VideoProcessingCompletedEvent(videoId, DateTime.UtcNow, zipKey);

        // Act
        await _handler.Handle(evt, CancellationToken.None);

        // Assert
        video.Status.Should().Be(Status.Completed);
        video.Width.Should().Be(3840);
        video.Height.Should().Be(2160);

        _mediatorMock.Verify(
            x => x.Publish(It.IsAny<SendEmailEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
