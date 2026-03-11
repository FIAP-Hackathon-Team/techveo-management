using Bogus;
using Microsoft.Extensions.DependencyInjection;
using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Management.Integration.Tests.Fixtures;
using TechVeo.Shared.Application.Storage;
using TechVeo.Shared.Domain.Exceptions;

namespace TechVeo.Management.Integration.Tests.Workflows;

public class VideoWorkflowTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly IVideoRepository _videoRepository;
    private readonly Mock<IVideoStorage> _videoStorageMock;
    private readonly Faker _faker;

    public VideoWorkflowTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepository = _fixture.ServiceProvider.GetRequiredService<IVideoRepository>();
        _videoStorageMock = new Mock<IVideoStorage>();
        _faker = new Faker();
    }

    [Fact(DisplayName = "Should create and retrieve video successfully")]
    [Trait("Integration", "VideoWorkflow")]
    public async Task VideoWorkflow_CreateAndRetrieve_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileName = _faker.System.FileName("mp4");
        var width = 1920;
        var height = 1080;
        var snapshotCount = 5;
        var intervalSeconds = 10.5;

        var video = new Domain.Entities.Video(userId, fileName, snapshotCount, intervalSeconds, width, height);

        // Act
        var videoId = await _videoRepository.AddAsync(video);
        await _fixture.DbContext.SaveChangesAsync();

        var retrievedVideo = await _videoRepository.GetByIdAsync(userId, videoId);

        // Assert
        retrievedVideo.Should().NotBeNull();
        retrievedVideo!.UserId.Should().Be(userId);
        retrievedVideo.FileName.Should().Be(fileName);
        retrievedVideo.Width.Should().Be(width);
        retrievedVideo.Height.Should().Be(height);
    }

    [Fact(DisplayName = "Should create multiple videos and retrieve all for a user")]
    [Trait("Integration", "VideoWorkflow")]
    public async Task VideoWorkflow_CreateMultipleAndRetrieveAll_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video1 = new Domain.Entities.Video(userId, "video1.mp4", 5, 10.5, 1920, 1080);
        var video2 = new Domain.Entities.Video(userId, "video2.mp4", 8, 15.0, 1280, 720);
        var video3 = new Domain.Entities.Video(userId, "video3.mp4", 10, 20.0, 3840, 2160);

        await _videoRepository.AddAsync(video1);
        await _videoRepository.AddAsync(video2);
        await _videoRepository.AddAsync(video3);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _videoRepository.GetByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(v => v.UserId.Should().Be(userId));
    }

    [Fact(DisplayName = "Should handle video upload workflow with file storage")]
    [Trait("Integration", "VideoWorkflow")]
    public async Task VideoWorkflow_UploadWithStorage_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileName = "workflow-video.mp4";
        var fileKey = "videos/workflow-video-123.mp4";
        var width = 1920;
        var height = 1080;

        var video = new Domain.Entities.Video(userId, fileName, null, null, width, height);
        video.SetFileKey(fileKey);

        // Act
        var videoId = await _videoRepository.AddAsync(video);
        await _fixture.DbContext.SaveChangesAsync();

        var retrievedVideo = await _videoRepository.GetByIdAsync(userId, videoId);

        // Assert
        retrievedVideo.Should().NotBeNull();
        retrievedVideo!.FileKey.Should().Be(fileKey);
        retrievedVideo.Status.Should().Be(Status.Queued);
        retrievedVideo.UserId.Should().Be(userId);
        retrievedVideo.FileName.Should().Be(fileName);
        retrievedVideo.Width.Should().Be(width);
        retrievedVideo.Height.Should().Be(height);
    }

    [Fact(DisplayName = "Should handle concurrent video uploads from different users")]
    [Trait("Integration", "VideoWorkflow")]
    public async Task VideoWorkflow_ConcurrentUploads_ShouldHandleSuccessfully()
    {
        // Arrange
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();
        var user3Id = Guid.NewGuid();

        // Add videos for user 1
        await _videoRepository.AddAsync(new Domain.Entities.Video(user1Id, "user1-video1.mp4", null, null, 1920, 1080));
        await _videoRepository.AddAsync(new Domain.Entities.Video(user1Id, "user1-video2.mp4", null, null, 1920, 1080));

        // Add videos for user 2
        await _videoRepository.AddAsync(new Domain.Entities.Video(user2Id, "user2-video1.mp4", null, null, 1280, 720));

        // Add videos for user 3
        await _videoRepository.AddAsync(new Domain.Entities.Video(user3Id, "user3-video1.mp4", null, null, 3840, 2160));
        await _videoRepository.AddAsync(new Domain.Entities.Video(user3Id, "user3-video2.mp4", null, null, 3840, 2160));
        await _videoRepository.AddAsync(new Domain.Entities.Video(user3Id, "user3-video3.mp4", null, null, 3840, 2160));

        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var results = await Task.WhenAll(
            _videoRepository.GetByUserIdAsync(user1Id),
            _videoRepository.GetByUserIdAsync(user2Id),
            _videoRepository.GetByUserIdAsync(user3Id)
        );

        // Assert
        results[0].Should().HaveCount(2);
        results[1].Should().HaveCount(1);
        results[2].Should().HaveCount(3);
        results.SelectMany(r => r).Should().HaveCount(6);
    }

    [Fact(DisplayName = "Should handle video workflow with snapshot generation")]
    [Trait("Integration", "VideoWorkflow")]
    public async Task VideoWorkflow_WithSnapshotGeneration_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video = new Domain.Entities.Video(userId, "snapshot-video.mp4", 10, 5.0, 1920, 1080);

        // Act
        var videoId = await _videoRepository.AddAsync(video);
        await _fixture.DbContext.SaveChangesAsync();

        var retrievedVideo = await _videoRepository.GetByIdAsync(userId, videoId);

        // Assert
        retrievedVideo.Should().NotBeNull();
        retrievedVideo!.SnapshotCount.Should().Be(10);
        retrievedVideo.IntervalSeconds.Should().Be(5.0);
    }

    [Fact(DisplayName = "Should validate video dimensions in workflow")]
    [Trait("Integration", "VideoWorkflow")]
    public void VideoWorkflow_ValidateDimensions_ShouldRejectInvalid()
    {
        // Arrange & Act & Assert
        var userId = Guid.NewGuid();
        Assert.Throws<DomainException>(() =>
            new Domain.Entities.Video(userId, "invalid.mp4", null, null, -100, 1080));
    }

    [Fact(DisplayName = "Should handle video with various resolutions")]
    [Trait("Integration", "VideoWorkflow")]
    public async Task VideoWorkflow_VariousResolutions_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resolutions = new List<(string name, int width, int height)>
        {
            ("480p", 854, 480),
            ("720p", 1280, 720),
            ("1080p", 1920, 1080),
            ("1440p", 2560, 1440),
            ("4K", 3840, 2160)
        };

        foreach (var (name, width, height) in resolutions)
        {
            var video = new Domain.Entities.Video(userId, $"{name}-video.mp4", null, null, width, height);
            await _videoRepository.AddAsync(video);
        }

        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _videoRepository.GetByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(5);
        result.Should().Contain(v => v.Width == 854 && v.Height == 480);
        result.Should().Contain(v => v.Width == 1280 && v.Height == 720);
        result.Should().Contain(v => v.Width == 1920 && v.Height == 1080);
        result.Should().Contain(v => v.Width == 2560 && v.Height == 1440);
        result.Should().Contain(v => v.Width == 3840 && v.Height == 2160);
    }
}
