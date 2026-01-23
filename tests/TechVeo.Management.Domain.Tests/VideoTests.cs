using TechVeo.Management.Domain.Tests.Fixtures;
using TechVeo.Shared.Domain.Exceptions;
using TechVeo.Management.Domain.Enums;

namespace TechVeo.Management.Domain.Tests
{
    public class VideoTests : IClassFixture<VideoFixture>
    {
        private readonly VideoFixture _videoFixture;

        public VideoTests(VideoFixture videoFixture)
        {
            _videoFixture = videoFixture;
        }

        [Fact(DisplayName = "Cannot create video with negative width")]
        [Trait("Domain", "Video Validation")]
        public void ShouldThrowException_WhenCreatingVideoWithNegativeWidth()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Domain.Entities.Video(userId, "test.mp4", null, null, -100, 1080));
        }

        [Fact(DisplayName = "Cannot create video with negative height")]
        [Trait("Domain", "Video Validation")]
        public void ShouldThrowException_WhenCreatingVideoWithNegativeHeight()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Domain.Entities.Video(userId, "test.mp4", null, null, 1920, -200));
        }

        [Fact(DisplayName = "Can create video with valid dimensions")]
        [Trait("Domain", "Video Creation")]
        public void ShouldCreateVideo_WithValidDimensions()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var video = _videoFixture.CreateValidVideo(userId);

            // Assert
            Assert.NotNull(video);
            Assert.Equal(userId, video.UserId);
            Assert.Equal(1920, video.Width);
            Assert.Equal(1080, video.Height);
            Assert.Equal(Status.Queued, video.Status);
        }

        [Fact(DisplayName = "Can set valid file key")]
        [Trait("Domain", "Video File")]
        public void ShouldSetFileKey_WithValidKey()
        {
            // Arrange
            var video = _videoFixture.CreateValidVideo(Guid.NewGuid());
            var fileKey = "videos/test-video-123.mp4";

            // Act
            video.SetFileKey(fileKey);

            // Assert
            Assert.Equal(fileKey, video.FileKey);
        }

        [Fact(DisplayName = "Cannot set empty file key")]
        [Trait("Domain", "Video File")]
        public void ShouldThrowException_WhenSettingEmptyFileKey()
        {
            // Arrange
            var video = _videoFixture.CreateValidVideo(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<DomainException>(() => video.SetFileKey(string.Empty));
        }

        [Fact(DisplayName = "Cannot set null file key")]
        [Trait("Domain", "Video File")]
        public void ShouldThrowException_WhenSettingNullFileKey()
        {
            // Arrange
            var video = _videoFixture.CreateValidVideo(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<DomainException>(() => video.SetFileKey(null!));
        }

        [Fact(DisplayName = "Can create video with snapshot parameters")]
        [Trait("Domain", "Video Creation")]
        public void ShouldCreateVideo_WithSnapshotParameters()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var video = _videoFixture.CreateValidVideo(userId);

            // Assert
            Assert.NotNull(video.SnapshotCount);
            Assert.NotNull(video.IntervalSeconds);
            Assert.Equal(5, video.SnapshotCount);
            Assert.Equal(10.5, video.IntervalSeconds);
        }

        [Fact(DisplayName = "Can create video without snapshot parameters")]
        [Trait("Domain", "Video Creation")]
        public void ShouldCreateVideo_WithoutSnapshotParameters()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var video = _videoFixture.CreateVideoWithoutSnapshots(userId);

            // Assert
            Assert.Null(video.SnapshotCount);
            Assert.Null(video.IntervalSeconds);
        }

        [Fact(DisplayName = "Can create 4K video")]
        [Trait("Domain", "Video Resolution")]
        public void ShouldCreateVideo_With4KResolution()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var video = _videoFixture.Create4KVideo(userId);

            // Assert
            Assert.Equal(3840, video.Width);
            Assert.Equal(2160, video.Height);
            Assert.Equal(20, video.SnapshotCount);
        }

        [Fact(DisplayName = "Can create HD video")]
        [Trait("Domain", "Video Resolution")]
        public void ShouldCreateVideo_WithHDResolution()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var video = _videoFixture.CreateHDVideo(userId);

            // Assert
            Assert.Equal(1280, video.Width);
            Assert.Equal(720, video.Height);
        }

        [Fact(DisplayName = "Video is created with Queued status by default")]
        [Trait("Domain", "Video Status")]
        public void VideoShouldBeCreatedWithQueuedStatus()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var video = _videoFixture.CreateValidVideo(userId);

            // Assert
            Assert.Equal(Status.Queued, video.Status);
        }

        [Fact(DisplayName = "Video is created with creation timestamp")]
        [Trait("Domain", "Video Metadata")]
        public void VideoShouldHaveCreationTimestamp()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var beforeCreation = DateTime.UtcNow;

            // Act
            var video = _videoFixture.CreateValidVideo(userId);
            var afterCreation = DateTime.UtcNow;

            // Assert
            Assert.NotEqual(DateTime.MinValue, video.CreateAt);
            Assert.True(beforeCreation <= video.CreateAt && video.CreateAt <= afterCreation);
        }

        [Fact(DisplayName = "Can update video width with valid value")]
        [Trait("Domain", "Video Dimension Update")]
        public void ShouldUpdateWidth_WithValidValue()
        {
            // Arrange
            var video = _videoFixture.CreateValidVideo(Guid.NewGuid());
            var newWidth = 2560;

            // Act
            video.SetWidth(newWidth);

            // Assert
            Assert.Equal(newWidth, video.Width);
        }

        [Fact(DisplayName = "Cannot update video width with negative value")]
        [Trait("Domain", "Video Dimension Update")]
        public void ShouldThrowException_WhenUpdatingWidthWithNegativeValue()
        {
            // Arrange
            var video = _videoFixture.CreateValidVideo(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<DomainException>(() => video.SetWidth(-100));
        }

        [Fact(DisplayName = "Can update video height with valid value")]
        [Trait("Domain", "Video Dimension Update")]
        public void ShouldUpdateHeight_WithValidValue()
        {
            // Arrange
            var video = _videoFixture.CreateValidVideo(Guid.NewGuid());
            var newHeight = 1440;

            // Act
            video.SetHeight(newHeight);

            // Assert
            Assert.Equal(newHeight, video.Height);
        }

        [Fact(DisplayName = "Cannot update video height with negative value")]
        [Trait("Domain", "Video Dimension Update")]
        public void ShouldThrowException_WhenUpdatingHeightWithNegativeValue()
        {
            // Arrange
            var video = _videoFixture.CreateValidVideo(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<DomainException>(() => video.SetHeight(-200));
        }
    }
}
