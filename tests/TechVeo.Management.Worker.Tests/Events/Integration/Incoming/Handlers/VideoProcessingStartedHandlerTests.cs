using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using TechVeo.Management.Application.Events.Integration.Incoming;
using TechVeo.Management.Application.Events.Integration.Incoming.Handlers;
using TechVeo.Management.Domain.Entities;
using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;
using Xunit;

public class VideoProcessingStartedHandlerTests
{
    [Fact]
    public async Task Handle_Should_Set_Status_To_Processing()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var video = new Video(Guid.NewGuid(), "file.mp4", 1, 1.0, 1920, 1080);

        var repoMock = new Mock<IVideoRepository>();
        repoMock.Setup(r => r.GetByIdAsync(videoId)).ReturnsAsync(video);

        var mediatorMock = new Mock<IMediator>();

        var handler = new VideoProcessingStartedHandler(repoMock.Object, mediatorMock.Object);
        var evt = new VideoProcessingStartedEvent(videoId, DateTime.UtcNow);

        // Act
        await handler.Handle(evt, CancellationToken.None);

        // Assert
        Assert.Equal(Status.Processing, video.Status);
    }
}
