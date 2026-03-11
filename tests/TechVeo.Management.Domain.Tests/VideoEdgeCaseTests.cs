using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Entities;
using TechVeo.Shared.Domain.Exceptions;

namespace TechVeo.Management.Domain.Tests;

public class VideoStatusTransitionTests
{
    [Fact(DisplayName = "Video can transition from Queued to Processing")]
    [Trait("Domain", "Video Status Transitions")]
    public void ShouldTransition_FromQueuedToProcessing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video = new Video(userId, "test.mp4", null, null, 1920, 1080);
        Assert.Equal(Status.Queued, video.Status);

        // Act
        video.SetStatus(Status.Processing);

        // Assert
        Assert.Equal(Status.Processing, video.Status);
    }

    [Fact(DisplayName = "Video can transition from Processing to Completed")]
    [Trait("Domain", "Video Status Transitions")]
    public void ShouldTransition_FromProcessingToCompleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video = new Video(userId, "test.mp4", null, null, 1920, 1080);
        video.SetStatus(Status.Processing);

        // Act
        video.SetStatus(Status.Completed);

        // Assert
        Assert.Equal(Status.Completed, video.Status);
    }

    [Fact(DisplayName = "Video can transition from Processing to Failed")]
    [Trait("Domain", "Video Status Transitions")]
    public void ShouldTransition_FromProcessingToFailed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video = new Video(userId, "test.mp4", null, null, 1920, 1080);
        video.SetStatus(Status.Processing);

        // Act
        video.SetStatus(Status.Failed);

        // Assert
        Assert.Equal(Status.Failed, video.Status);
    }

    [Fact(DisplayName = "Video maintains different statuses correctly")]
    [Trait("Domain", "Video Status Management")]
    public void ShouldMaintainDifferentStatuses()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var video1 = new Video(userId1, "video1.mp4", null, null, 1920, 1080);
        var video2 = new Video(userId2, "video2.mp4", null, null, 1280, 720);

        // Act
        video1.SetStatus(Status.Completed);
        video2.SetStatus(Status.Failed);

        // Assert
        Assert.Equal(Status.Completed, video1.Status);
        Assert.Equal(Status.Failed, video2.Status);
    }
}

public class VideoFileKeyManagementTests
{
    [Fact(DisplayName = "Video fileKey is null when created")]
    [Trait("Domain", "Video File Management")]
    public void VideoFileKeyIsNull_WhenCreated()
    {
        // Arrange & Act
        var video = new Video(Guid.NewGuid(), "test.mp4", null, null, 1920, 1080);

        // Assert
        Assert.Null(video.FileKey);
    }

    [Fact(DisplayName = "Can set fileKey multiple times")]
    [Trait("Domain", "Video File Management")]
    public void CanSetFileKey_MultipleTimes()
    {
        // Arrange
        var video = new Video(Guid.NewGuid(), "test.mp4", null, null, 1920, 1080);
        var firstKey = "videos/123.mp4";
        var secondKey = "videos/updated/123.mp4";

        // Act
        video.SetFileKey(firstKey);
        var firstResult = video.FileKey;
        video.SetFileKey(secondKey);
        var secondResult = video.FileKey;

        // Assert
        Assert.Equal(firstKey, firstResult);
        Assert.Equal(secondKey, secondResult);
    }

    [Fact(DisplayName = "FileKey cannot be whitespace")]
    [Trait("Domain", "Video File Management")]
    public void FileKeyCannotBeWhitespace()
    {
        // Arrange
        var video = new Video(Guid.NewGuid(), "test.mp4", null, null, 1920, 1080);

        // Act & Assert
        Assert.Throws<DomainException>(() => video.SetFileKey("   "));
    }
}

public class VideoDimensionTests
{
    [Fact(DisplayName = "Video dimensions must be non-negative")]
    [Trait("Domain", "Video Dimensions")]
    public void VideoWidthAndHeightMustBeNonNegative()
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() =>
            new Video(Guid.NewGuid(), "test.mp4", null, null, -1, 1080));

        Assert.Throws<DomainException>(() =>
            new Video(Guid.NewGuid(), "test.mp4", null, null, 1920, -1));
    }

    [Fact(DisplayName = "Video supports zero dimensions")]
    [Trait("Domain", "Video Dimensions")]
    public void VideoSupportsZeroDimensions()
    {
        // Arrange & Act & Assert
        var video = new Video(Guid.NewGuid(), "test.mp4", null, null, 0, 0);
        Assert.Equal(0, video.Width);
        Assert.Equal(0, video.Height);
    }

    [Fact(DisplayName = "Video supports very large dimensions")]
    [Trait("Domain", "Video Dimensions")]
    public void VideoSupportsLargeDimensions()
    {
        // Arrange
        var largeWidth = 10000;
        var largeHeight = 10000;

        // Act
        var video = new Video(Guid.NewGuid(), "test.mp4", null, null, largeWidth, largeHeight);

        // Assert
        Assert.Equal(largeWidth, video.Width);
        Assert.Equal(largeHeight, video.Height);
    }

    [Fact(DisplayName = "Video can be updated to different dimensions")]
    [Trait("Domain", "Video Dimensions")]
    public void CanUpdateVideoDimensions()
    {
        // Arrange
        var video = new Video(Guid.NewGuid(), "test.mp4", null, null, 1920, 1080);

        // Act
        video.SetWidth(2560);
        video.SetHeight(1440);

        // Assert
        Assert.Equal(2560, video.Width);
        Assert.Equal(1440, video.Height);
    }
}

public class VideoSnapshotParametersTests
{
    [Fact(DisplayName = "Video with snapshot parameters stores them correctly")]
    [Trait("Domain", "Video Snapshot Parameters")]
    public void SnapshotParametersStoredCorrectly()
    {
        // Arrange & Act
        var video = new Video(Guid.NewGuid(), "test.mp4", 10, 5.5, 1920, 1080);

        // Assert
        Assert.Equal(10, video.SnapshotCount);
        Assert.Equal(5.5, video.IntervalSeconds);
    }

    [Fact(DisplayName = "Video without snapshot parameters is valid")]
    [Trait("Domain", "Video Snapshot Parameters")]
    public void VideoWithoutSnapshotParametersIsValid()
    {
        // Arrange & Act
        var video = new Video(Guid.NewGuid(), "test.mp4", null, null, 1920, 1080);

        // Assert
        Assert.Null(video.SnapshotCount);
        Assert.Null(video.IntervalSeconds);
    }

    [Fact(DisplayName = "Video snapshot parameters can be zero")]
    [Trait("Domain", "Video Snapshot Parameters")]
    public void SnapshotParametersCanBeZero()
    {
        // Arrange & Act
        var video = new Video(Guid.NewGuid(), "test.mp4", 0, 0.0, 1920, 1080);

        // Assert
        Assert.Equal(0, video.SnapshotCount);
        Assert.Equal(0.0, video.IntervalSeconds);
    }

    [Fact(DisplayName = "Video supports negative snapshot counts (edge case)")]
    [Trait("Domain", "Video Snapshot Parameters")]
    public void VideoSupportsNegativeSnapshotCounts()
    {
        // This tests current behavior - whether negative snapshots are allowed
        // Arrange & Act
        var video = new Video(Guid.NewGuid(), "test.mp4", -5, -10.0, 1920, 1080);

        // Assert
        Assert.Equal(-5, video.SnapshotCount);
        Assert.Equal(-10.0, video.IntervalSeconds);
    }
}
