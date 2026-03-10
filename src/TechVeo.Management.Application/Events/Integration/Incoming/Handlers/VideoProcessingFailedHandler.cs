using MediatR;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Events.Integration.Incoming.Handlers
{
    internal class VideoProcessingFailedHandler(
        IVideoRepository repo,
        IMediator mediator) : INotificationHandler<VideoProcessingFailedEvent>
    {
        public async Task Handle(VideoProcessingFailedEvent notification, CancellationToken cancellationToken)
        {
            var video = await repo.GetByIdAsync(notification.VideoId);

            if (video is null)
                return;

            video.GetType().GetProperty("Status")?.SetValue(video, Domain.Enums.Status.Failed);

            await mediator.Publish(new SendEmailEvent
            (
                video.EmailAddress,
                video.FileName ?? "",
                Domain.Enums.Status.Failed,
                ""
            ), cancellationToken);
        }
    }
}
