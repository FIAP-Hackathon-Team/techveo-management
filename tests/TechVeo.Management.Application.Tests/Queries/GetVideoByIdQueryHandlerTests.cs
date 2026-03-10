using TechVeo.Management.Application.Queries.GetVideoById;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Management.Domain.Entities;
using TechVeo.Management.Domain.Enums;

namespace TechVeo.Management.Application.Tests.Queries;

public class GetVideoByIdQueryHandlerTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;

    public GetVideoByIdQueryHandlerTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
    }

    [Fact(DisplayName = "Handle should return VideoDto when video exists")]
    [Trait("Application", "GetVideoByIdQueryHandler")]
    public async Task Handle_WithExistingVideo_ShouldReturnVideoDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video = new Video(userId, "video.mp4", 5, 10.5, 1920, 1080);

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, video.Id))
            .ReturnsAsync(video);

        var handler = new GetVideoByIdQueryHandler(_videoRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetVideoByIdQuery(userId, video.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(video.Id);
        result.FileName.Should().Be(video.FileName);
        result.Width.Should().Be(video.Width);
        result.Height.Should().Be(video.Height);
        result.SnapshotCount.Should().Be(video.SnapshotCount);
        result.IntervalSeconds.Should().Be(video.IntervalSeconds);
        result.Status.Should().Be(video.Status);

        _videoRepositoryMock.Verify(x => x.GetByIdAsync(userId, video.Id), Times.Once);
    }

    [Fact(DisplayName = "Handle should return null when video does not exist")]
    [Trait("Application", "GetVideoByIdQueryHandler")]
    public async Task Handle_WithNonExistingVideo_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var videoId = Guid.NewGuid();

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, videoId))
            .ReturnsAsync((Video?)null);

        var handler = new GetVideoByIdQueryHandler(_videoRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetVideoByIdQuery(userId, videoId), CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _videoRepositoryMock.Verify(x => x.GetByIdAsync(userId, videoId), Times.Once);
    }
}
