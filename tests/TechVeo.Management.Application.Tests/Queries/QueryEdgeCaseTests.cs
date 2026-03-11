using TechVeo.Management.Application.Dto;
using TechVeo.Management.Application.Queries.Video.GetAllVideos;
using TechVeo.Management.Application.Queries.Video.GetVideoById;
using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Tests.Queries;

public class QueryValidationTests
{
    [Fact(DisplayName = "GetAllVideosQuery should preserve user ID")]
    [Trait("Application", "GetAllVideosQuery")]
    public void GetAllVideosQuery_ShouldPreserveUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var query = new GetAllVideosQuery(userId);

        // Assert
        query.UserId.Should().Be(userId);
    }

    [Fact(DisplayName = "GetVideoByIdQuery should preserve both IDs")]
    [Trait("Application", "GetVideoByIdQuery")]
    public void GetVideoByIdQuery_ShouldPreserveBothIds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var videoId = Guid.NewGuid();

        // Act
        var query = new GetVideoByIdQuery(userId, videoId);

        // Assert
        query.UserId.Should().Be(userId);
        query.VideoId.Should().Be(videoId);
    }
}

public class GetAllVideosQueryHandlerEdgeCaseTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly GetAllVideosQueryHandler _handler;

    public GetAllVideosQueryHandlerEdgeCaseTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _handler = new GetAllVideosQueryHandler(_videoRepositoryMock.Object);
    }

    [Fact(DisplayName = "Should handle user with very large number of videos")]
    [Trait("Application", "GetAllVideosQueryHandler")]
    public async Task Handle_WithManyVideos_ShouldReturnAll()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var videoCount = 1000;
        var videos = new List<Domain.Entities.Video>();
        
        for (int i = 0; i < videoCount; i++)
        {
            videos.Add(new Domain.Entities.Video(
                userId, 
                $"video{i}.mp4", 
                i % 2 == 0 ? 5 : null, 
                i % 2 == 0 ? 10.5 : null, 
                1920, 
                1080));
        }

        _videoRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(videos);

        var query = new GetAllVideosQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(videoCount);
        result.Should().AllSatisfy(v => v.Should().BeOfType<VideoDto>());
    }

    [Fact(DisplayName = "Should preserve all video properties in mapping")]
    [Trait("Application", "GetAllVideosQueryHandler")]
    public async Task Handle_ShouldPreserveAllVideoProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video = new Domain.Entities.Video(
            userId, 
            "detailed-video.mp4", 
            42, 
            3.14, 
            2560, 
            1440);
        video.SetStatus(Status.Completed);
        video.SetFileKey("videos/detailed-123.mp4");

        _videoRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Domain.Entities.Video> { video });

        var query = new GetAllVideosQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var mappedVideo = result[0];
        mappedVideo.Id.Should().Be(video.Id);
        mappedVideo.FileName.Should().Be("detailed-video.mp4");
        mappedVideo.SnapshotCount.Should().Be(42);
        mappedVideo.IntervalSeconds.Should().Be(3.14);
        mappedVideo.Width.Should().Be(2560);
        mappedVideo.Height.Should().Be(1440);
        mappedVideo.Status.Should().Be(Status.Completed);
    }

    [Fact(DisplayName = "Should handle mixed video statuses")]
    [Trait("Application", "GetAllVideosQueryHandler")]
    public async Task Handle_WithMixedStatuses_ShouldReturnAll()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var videos = new List<Domain.Entities.Video>();
        
        var queuedVideo = new Domain.Entities.Video(userId, "queued.mp4", null, null, 1920, 1080);
        var processingVideo = new Domain.Entities.Video(userId, "processing.mp4", null, null, 1280, 720);
        processingVideo.SetStatus(Status.Processing);
        var completedVideo = new Domain.Entities.Video(userId, "completed.mp4", 10, 5.0, 3840, 2160);
        completedVideo.SetStatus(Status.Completed);
        var failedVideo = new Domain.Entities.Video(userId, "failed.mp4", null, null, 1920, 1080);
        failedVideo.SetStatus(Status.Failed);

        videos.AddRange(new[] { queuedVideo, processingVideo, completedVideo, failedVideo });

        _videoRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(videos);

        var query = new GetAllVideosQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(v => v.Status == Status.Queued);
        result.Should().Contain(v => v.Status == Status.Processing);
        result.Should().Contain(v => v.Status == Status.Completed);
        result.Should().Contain(v => v.Status == Status.Failed);
    }
}

public class GetVideoByIdQueryHandlerEdgeCaseTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly GetVideoByIdQueryHandler _handler;

    public GetVideoByIdQueryHandlerEdgeCaseTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _handler = new GetVideoByIdQueryHandler(_videoRepositoryMock.Object);
    }

    [Fact(DisplayName = "Should return null for different user")]
    [Trait("Application", "GetVideoByIdQueryHandler")]
    public async Task Handle_WithDifferentUser_ShouldReturnNull()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var videoId = Guid.NewGuid();

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(userId1, videoId))
            .ReturnsAsync((Domain.Entities.Video?)null);

        var query = new GetVideoByIdQuery(userId1, videoId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "Should handle video with all optional properties")]
    [Trait("Application", "GetVideoByIdQueryHandler")]
    public async Task Handle_WithCompleteVideoData_ShouldMapAllProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video = new Domain.Entities.Video(
            userId, 
            "complete-video.mp4", 
            50, 
            2.5, 
            4096, 
            2304);
        var videoId = video.Id; // Use the auto-generated ID
        video.SetStatus(Status.Completed);
        video.SetFileKey("videos/complete-123.mp4");

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, videoId))
            .ReturnsAsync(video);

        var query = new GetVideoByIdQuery(userId, videoId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(videoId);
        result.FileName.Should().Be("complete-video.mp4");
        result.SnapshotCount.Should().Be(50);
        result.IntervalSeconds.Should().Be(2.5);
        result.Width.Should().Be(4096);
        result.Height.Should().Be(2304);
        result.Status.Should().Be(Status.Completed);
    }

    [Fact(DisplayName = "Should handle video with minimal properties")]
    [Trait("Application", "GetVideoByIdQueryHandler")]
    public async Task Handle_WithMinimalVideoData_ShouldMapCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var videoId = Guid.NewGuid();
        var video = new Domain.Entities.Video(
            userId, 
            null, 
            null, 
            null, 
            0, 
            0);

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, videoId))
            .ReturnsAsync(video);

        var query = new GetVideoByIdQuery(userId, videoId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.FileName.Should().BeNull();
        result.SnapshotCount.Should().BeNull();
        result.IntervalSeconds.Should().BeNull();
        result.Width.Should().Be(0);
        result.Height.Should().Be(0);
    }

    [Fact(DisplayName = "Should call repository with correct parameters")]
    [Trait("Application", "GetVideoByIdQueryHandler")]
    public async Task Handle_ShouldCallRepositoryWithCorrectParams()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var videoId = Guid.NewGuid();

        _videoRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, videoId))
            .ReturnsAsync((Domain.Entities.Video?)null);

        var query = new GetVideoByIdQuery(userId, videoId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _videoRepositoryMock.Verify(
            x => x.GetByIdAsync(userId, videoId),
            Times.Once);
    }
}
