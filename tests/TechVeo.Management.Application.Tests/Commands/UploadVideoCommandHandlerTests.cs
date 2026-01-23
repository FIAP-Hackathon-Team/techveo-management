using MediatR;
using Microsoft.AspNetCore.Http;
using TechVeo.Management.Application.Commands.Video.Upload;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Management.Domain.Enums;
using TechVeo.Shared.Application.Storage;

namespace TechVeo.Management.Application.Tests.Commands;

public class UploadVideoCommandHandlerTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IVideoStorage> _videoStorageMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetAllVideosByUserIdCommandHandler _handler;

    public UploadVideoCommandHandlerTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _videoStorageMock = new Mock<IVideoStorage>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new GetAllVideosByUserIdCommandHandler(
            _videoRepositoryMock.Object,
            _videoStorageMock.Object,
            _mediatorMock.Object);
    }

    [Fact(DisplayName = "Should upload video successfully with valid file")]
    [Trait("Application", "UploadVideoCommandHandler")]
    public async Task Handle_WithValidFile_ShouldUploadVideoSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileName = "test-video.mp4";
        var width = 1920;
        var height = 1080;
        var snapshotCount = 5;
        var intervalSeconds = 10.5;
        var fileKey = "videos/test-video-123.mp4";
        var videoId = Guid.NewGuid();

        var fileContent = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var command = new UploadVideoCommand(userId, formFile, snapshotCount, intervalSeconds, width, height);

        _videoStorageMock
            .Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileKey);

        _videoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Video>()))
            .ReturnsAsync(videoId);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Width.Should().Be(width);
        result.Height.Should().Be(height);
        result.SnapshotCount.Should().Be(snapshotCount);
        result.IntervalSeconds.Should().Be(intervalSeconds);
        result.Status.Should().Be(Status.Queued);

        _videoStorageMock.Verify(
            x => x.UploadVideoAsync(It.IsAny<Stream>(), fileName, It.IsAny<CancellationToken>()),
            Times.Once);

        _videoRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Entities.Video>(v =>
                v.UserId == userId &&
                v.FileName == fileName &&
                v.Width == width &&
                v.Height == height &&
                v.SnapshotCount == snapshotCount &&
                v.IntervalSeconds == intervalSeconds)),
            Times.Once);

        _mediatorMock.Verify(
            x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should upload video with optional snapshot parameters")]
    [Trait("Application", "UploadVideoCommandHandler")]
    public async Task Handle_WithoutSnapshotParameters_ShouldUploadVideoSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileName = "test-video2.mp4";
        var width = 1280;
        var height = 720;
        var fileKey = "videos/test-video2-123.mp4";
        var videoId = Guid.NewGuid();

        var fileContent = new MemoryStream(new byte[] { 1, 2, 3 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var command = new UploadVideoCommand(userId, formFile, null, null, width, height);

        _videoStorageMock
            .Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileKey);

        _videoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Video>()))
            .ReturnsAsync(videoId);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.SnapshotCount.Should().BeNull();
        result.IntervalSeconds.Should().BeNull();
        result.Status.Should().Be(Status.Queued);
    }

    [Fact(DisplayName = "Should upload video with 4K resolution")]
    [Trait("Application", "UploadVideoCommandHandler")]
    public async Task Handle_With4KResolution_ShouldUploadVideoSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileName = "4k-video.mp4";
        var width = 3840;
        var height = 2160;
        var snapshotCount = 20;
        var intervalSeconds = 5.0;
        var fileKey = "videos/4k-video-123.mp4";
        var videoId = Guid.NewGuid();

        var fileContent = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var command = new UploadVideoCommand(userId, formFile, snapshotCount, intervalSeconds, width, height);

        _videoStorageMock
            .Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileKey);

        _videoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Video>()))
            .ReturnsAsync(videoId);

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Width.Should().Be(3840);
        result.Height.Should().Be(2160);
        result.SnapshotCount.Should().Be(20);
    }
}
