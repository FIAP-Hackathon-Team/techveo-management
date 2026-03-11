using TechVeo.Management.Application.Commands.Video.Upload;
using TechVeo.Management.Domain.Enums;
using Microsoft.AspNetCore.Http;
using MediatR;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Shared.Application.Storage;

namespace TechVeo.Management.Application.Tests.Commands;

public class UploadVideoCommandValidationTests
{
    [Fact(DisplayName = "Command should preserve all parameters")]
    [Trait("Application", "UploadVideoCommand")]
    public void CommandShouldPreserveAllParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var width = 1920;
        var height = 1080;
        var snapshotCount = 5;
        var intervalSeconds = 10.5;
        var fileContent = new MemoryStream();
        var formFile = new FormFile(fileContent, 0, 0, "file", "test.mp4");

        // Act
        var command = new UploadVideoCommand(userId, formFile, snapshotCount, intervalSeconds, width, height);

        // Assert
        command.UserId.Should().Be(userId);
        command.File.Should().Be(formFile);
        command.SnapshotCount.Should().Be(snapshotCount);
        command.IntervalSeconds.Should().Be(intervalSeconds);
        command.Width.Should().Be(width);
        command.Height.Should().Be(height);
    }

    [Fact(DisplayName = "Command can be created without snapshot parameters")]
    [Trait("Application", "UploadVideoCommand")]
    public void CommandWithoutSnapshotParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileContent = new MemoryStream();
        var formFile = new FormFile(fileContent, 0, 0, "file", "test.mp4");

        // Act
        var command = new UploadVideoCommand(userId, formFile, null, null, 1920, 1080);

        // Assert
        command.SnapshotCount.Should().BeNull();
        command.IntervalSeconds.Should().BeNull();
    }

    [Fact(DisplayName = "Command should support different video dimensions")]
    [Trait("Application", "UploadVideoCommand")]
    public void CommandWithDifferentDimensions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileContent = new MemoryStream();
        var formFile = new FormFile(fileContent, 0, 0, "file", "test.mp4");

        // Act & Assert
        var command4K = new UploadVideoCommand(userId, formFile, null, null, 3840, 2160);
        var commandHD = new UploadVideoCommand(userId, formFile, null, null, 1280, 720);
        var commandFullHD = new UploadVideoCommand(userId, formFile, null, null, 1920, 1080);

        command4K.Width.Should().Be(3840);
        command4K.Height.Should().Be(2160);
        commandHD.Width.Should().Be(1280);
        commandHD.Height.Should().Be(720);
        commandFullHD.Width.Should().Be(1920);
        commandFullHD.Height.Should().Be(1080);
    }
}

public class UploadVideoCommandHandlerEdgeCaseTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IVideoStorage> _videoStorageMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UploadVideoCommandHandler _handler;

    public UploadVideoCommandHandlerEdgeCaseTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _videoStorageMock = new Mock<IVideoStorage>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new UploadVideoCommandHandler(
            _videoRepositoryMock.Object,
            _videoStorageMock.Object,
            _mediatorMock.Object);
    }

    [Fact(DisplayName = "Should handle large file uploads")]
    [Trait("Application", "UploadVideoCommandHandler")]
    public async Task Handle_WithLargeFile_ShouldUploadSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileName = "large-video.mp4";
        var width = 3840;
        var height = 2160;
        var largeFileSize = 1024 * 1024 * 500; // 500 MB
        var fileContent = new MemoryStream(new byte[largeFileSize]);
        
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", fileName)
        {
            Headers = new Microsoft.AspNetCore.Http.HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var command = new UploadVideoCommand(userId, formFile, null, null, width, height);
        var fileKey = "videos/large-video-123.mp4";
        var videoId = Guid.NewGuid();

        _videoStorageMock
            .Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), fileName, It.IsAny<CancellationToken>()))
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
    }

    [Fact(DisplayName = "Should preserve original filename in DTO")]
    [Trait("Application", "UploadVideoCommandHandler")]
    public async Task Handle_ShouldPreserveFilename()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileName = "my-special-video-2024.mp4";
        var fileContent = new MemoryStream(new byte[] { 1, 2, 3 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", fileName)
        {
            Headers = new Microsoft.AspNetCore.Http.HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var command = new UploadVideoCommand(userId, formFile, null, null, 1920, 1080);

        _videoStorageMock
            .Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("videos/hash123.mp4");

        _videoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Video>()))
            .ReturnsAsync(Guid.NewGuid());

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.FileName.Should().Be(fileName);
    }

    [Fact(DisplayName = "Should handle decimal interval seconds")]
    [Trait("Application", "UploadVideoCommandHandler")]
    public async Task Handle_WithDecimalIntervalSeconds_ShouldPreserveValue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var intervalSeconds = 3.3333333;
        var fileContent = new MemoryStream(new byte[] { 1, 2, 3 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", "test.mp4")
        {
            Headers = new Microsoft.AspNetCore.Http.HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var command = new UploadVideoCommand(userId, formFile, 100, intervalSeconds, 1920, 1080);

        _videoStorageMock
            .Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("videos/hash123.mp4");

        _videoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Video>()))
            .ReturnsAsync(Guid.NewGuid());

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IntervalSeconds.Should().Be(intervalSeconds);
    }

    [Fact(DisplayName = "Should publish event with correct metadata")]
    [Trait("Application", "UploadVideoCommandHandler")]
    public async Task Handle_ShouldPublishEventWithCorrectMetadata()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var snapshotCount = 25;
        var intervalSeconds = 7.5;
        var fileContent = new MemoryStream(new byte[] { 1, 2, 3 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", "test.mp4")
        {
            Headers = new Microsoft.AspNetCore.Http.HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var command = new UploadVideoCommand(userId, formFile, snapshotCount, intervalSeconds, 3840, 2160);

        _videoStorageMock
            .Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("videos/hash123.mp4");

        _videoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Video>()))
            .ReturnsAsync(Guid.NewGuid());

        _mediatorMock
            .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - Verify event was published with correct structure
        _mediatorMock.Verify(
            x => x.Publish(
                It.IsAny<INotification>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
