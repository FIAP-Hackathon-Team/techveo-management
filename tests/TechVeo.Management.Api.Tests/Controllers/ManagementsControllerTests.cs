using MediatR;
using Microsoft.AspNetCore.Http;
using TechVeo.Management.Api.Controllers;
using TechVeo.Management.Application.Commands.Video.Upload;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Contracts.Managements;
using TechVeo.Management.Domain.Enums;
using System.Security.Claims;
using TechVeo.Management.Application.Queries.Video.GetAllVideos;
using TechVeo.Management.Application.Queries.Video.GetVideoById;
using Microsoft.AspNetCore.Mvc;

namespace TechVeo.Management.Api.Tests.Controllers;

public class ManagementsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ManagementsController _controller;

    public ManagementsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ManagementsController(_mediatorMock.Object);
        
        // Setup mock user context with required "sub" claim
        var userId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("sub", userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "TestAuthType"));
        
        _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact(DisplayName = "CreateAsync should return Ok with created video")]
    [Trait("Api", "ManagementsController")]
    public async Task CreateAsync_WithValidRequest_ShouldReturnOkWithVideo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileName = "test-video.mp4";
        var width = 1920;
        var height = 1080;
        var snapshotCount = 5;
        var intervalSeconds = 10.5;

        var fileContent = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var request = new UploadVideoRequest(formFile, snapshotCount, intervalSeconds, width, height);

        var expectedVideo = new VideoDto(
            Guid.NewGuid(),
            Status.Queued,
            fileName,
            intervalSeconds,
            snapshotCount,
            width,
            height);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<UploadVideoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVideo);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedVideo);

        _mediatorMock.Verify(x => x.Send(
            It.Is<UploadVideoCommand>(cmd =>
                cmd.File.FileName == fileName &&
                cmd.Width == width &&
                cmd.Height == height &&
                cmd.SnapshotCount == snapshotCount),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "GetAllAsync should return Ok with list of videos")]
    [Trait("Api", "ManagementsController")]
    public async Task GetAllAsync_ShouldReturnOkWithVideosList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedVideos = new List<VideoDto>
        {
            new VideoDto(Guid.NewGuid(), Status.Queued, "video1.mp4", 10.5, 5, 1920, 1080),
            new VideoDto(Guid.NewGuid(), Status.Processing, "video2.mp4", 15.0, 8, 1280, 720),
            new VideoDto(Guid.NewGuid(), Status.Completed, "video3.mp4", 20.0, 10, 3840, 2160)
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetAllVideosQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVideos);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedVideos);

        _mediatorMock.Verify(x => x.Send(
            It.IsAny<GetAllVideosQuery>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "GetAllAsync should return empty list when user has no videos")]
    [Trait("Api", "ManagementsController")]
    public async Task GetAllAsync_WithNoVideos_ShouldReturnEmptyList()
    {
        // Arrange
        var expectedVideos = new List<VideoDto>();

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetAllVideosQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVideos);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var videos = okResult!.Value as List<VideoDto>;
        videos.Should().BeEmpty();
    }

    [Fact(DisplayName = "CreateAsync should validate video dimensions")]
    [Trait("Api", "ManagementsController")]
    public async Task CreateAsync_WithVariousDimensions_ShouldHandleCorrectly()
    {
        // Arrange
        var fileContent = new MemoryStream(new byte[] { 1, 2, 3 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", "test.mp4")
        {
            Headers = new HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var request = new UploadVideoRequest(formFile, 10, 5.0, 3840, 2160); // 4K resolution

        var expectedVideo = new VideoDto(
            Guid.NewGuid(),
            Status.Queued,
            "test.mp4",
            5.0,
            10,
            3840,
            2160);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<UploadVideoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVideo);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedVideo);
    }

    [Fact(DisplayName = "GetByIdAsync should return video when it exists")]
    [Trait("Api", "ManagementsController")]
    public async Task GetByIdAsync_WithExistingVideo_ShouldReturnOkWithVideo()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var expectedVideo = new VideoDto(
            videoId,
            Status.Completed,
            "existing-video.mp4",
            10.5,
            5,
            1920,
            1080);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetVideoByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVideo);

        // Act
        var result = await _controller.GetByIdAsync(videoId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedVideo);

        _mediatorMock.Verify(x => x.Send(
            It.Is<GetVideoByIdQuery>(q => q.VideoId == videoId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "GetByIdAsync should return NotFound when video doesn't exist")]
    [Trait("Api", "ManagementsController")]
    public async Task GetByIdAsync_WithNonExistentVideo_ShouldReturnNotFound()
    {
        // Arrange
        var videoId = Guid.NewGuid();

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetVideoByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((VideoDto?)null);

        // Act
        var result = await _controller.GetByIdAsync(videoId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        _mediatorMock.Verify(x => x.Send(
            It.Is<GetVideoByIdQuery>(q => q.VideoId == videoId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "CreateAsync should handle video without snapshot parameters")]
    [Trait("Api", "ManagementsController")]
    public async Task CreateAsync_WithoutSnapshotParameters_ShouldReturnOk()
    {
        // Arrange
        var fileName = "simple-video.mp4";
        var fileContent = new MemoryStream(new byte[] { 1, 2, 3 });
        var formFile = new FormFile(fileContent, 0, fileContent.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var request = new UploadVideoRequest(formFile, null, null, 1280, 720);

        var expectedVideo = new VideoDto(
            Guid.NewGuid(),
            Status.Queued,
            fileName,
            null,
            null,
            1280,
            720);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<UploadVideoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVideo);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var videoResult = okResult!.Value as VideoDto;
        videoResult!.SnapshotCount.Should().BeNull();
        videoResult!.IntervalSeconds.Should().BeNull();
    }

    [Fact(DisplayName = "GetAllAsync should return videos with different statuses")]
    [Trait("Api", "ManagementsController")]
    public async Task GetAllAsync_WithDifferentVideoStatuses_ShouldReturnAllVideos()
    {
        // Arrange
        var expectedVideos = new List<VideoDto>
        {
            new VideoDto(Guid.NewGuid(), Status.Queued, "queued.mp4", null, null, 1920, 1080),
            new VideoDto(Guid.NewGuid(), Status.Processing, "processing.mp4", 5.0, 10, 1280, 720),
            new VideoDto(Guid.NewGuid(), Status.Completed, "completed.mp4", 10.0, 20, 3840, 2160),
            new VideoDto(Guid.NewGuid(), Status.Failed, "failed.mp4", null, null, 1920, 1080)
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetAllVideosQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVideos);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var videos = okResult!.Value as List<VideoDto>;
        videos.Should().HaveCount(4);
        videos!.Should().Contain(v => v.Status == Status.Queued);
        videos.Should().Contain(v => v.Status == Status.Processing);
        videos.Should().Contain(v => v.Status == Status.Completed);
        videos.Should().Contain(v => v.Status == Status.Failed);
    }
}
