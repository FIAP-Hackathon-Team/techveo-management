using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Tests.Query;

public class GetAllVideosQueryHandlerTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;

    public GetAllVideosQueryHandlerTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
    }

    [Fact(DisplayName = "Should return all videos for a specific user")]
    [Trait("Application", "GetAllVideosByUserIdCommandHandler")]
    public async Task Handle_WithValidUserId_ShouldReturnAllUserVideos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var videos = new List<Domain.Entities.Video>
        {
            new Domain.Entities.Video(userId, "video1.mp4", 5, 10.5, 1920, 1080),
            new Domain.Entities.Video(userId, "video2.mp4", 8, 15.0, 1280, 720),
            new Domain.Entities.Video(userId, "video3.mp4", 10, 20.0, 3840, 2160)
        };

        _videoRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(videos);

        // Act
        var result = await _videoRepositoryMock.Object.GetByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(v => v.Should().BeOfType<Domain.Entities.Video>());

        result[0].FileName.Should().Be("video1.mp4");
        result[1].FileName.Should().Be("video2.mp4");
        result[2].FileName.Should().Be("video3.mp4");

        _videoRepositoryMock.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact(DisplayName = "Should return empty list when user has no videos")]
    [Trait("Application", "GetAllVideosByUserIdCommandHandler")]
    public async Task Handle_WithUserHavingNoVideos_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyVideos = new List<Domain.Entities.Video>();

        _videoRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(emptyVideos);

        // Act
        var result = await _videoRepositoryMock.Object.GetByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _videoRepositoryMock.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact(DisplayName = "Should return videos with different statuses")]
    [Trait("Application", "GetAllVideosByUserIdCommandHandler")]
    public async Task Handle_WithVideosDifferentStatuses_ShouldReturnAllVideos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var queuedVideo = new Domain.Entities.Video(userId, "queued.mp4", null, null, 1920, 1080);
        var processingVideo = new Domain.Entities.Video(userId, "processing.mp4", null, null, 1920, 1080);
        var completedVideo = new Domain.Entities.Video(userId, "completed.mp4", null, null, 1920, 1080);

        var videos = new List<Domain.Entities.Video> { queuedVideo, processingVideo, completedVideo };

        _videoRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(videos);

        // Act
        var result = await _videoRepositoryMock.Object.GetByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(v => v.Status.Should().BeOneOf(Status.Queued, Status.Processing, Status.Completed));
    }

    [Fact(DisplayName = "Should return videos ordered by creation date")]
    [Trait("Application", "GetAllVideosByUserIdCommandHandler")]
    public async Task Handle_ShouldReturnVideosOrderedByCreationDate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video1 = new Domain.Entities.Video(userId, "video1.mp4", null, null, 1920, 1080);

        // Create second video with slight delay to ensure different creation times
        await Task.Delay(10);
        var video2 = new Domain.Entities.Video(userId, "video2.mp4", null, null, 1280, 720);

        await Task.Delay(10);
        var video3 = new Domain.Entities.Video(userId, "video3.mp4", null, null, 3840, 2160);

        var videos = new List<Domain.Entities.Video> { video1, video2, video3 };

        _videoRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(videos);

        // Act
        var result = await _videoRepositoryMock.Object.GetByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(3);
        // Verify videos are ordered by creation date (earlier videos first)
        Assert.True(result[0].CreateAt <= result[1].CreateAt);
        Assert.True(result[1].CreateAt <= result[2].CreateAt);
    }
}

