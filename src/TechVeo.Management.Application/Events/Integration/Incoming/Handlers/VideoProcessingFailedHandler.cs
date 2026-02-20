using MediatR;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Events.Integration.Incoming.Handlers
{
    internal class VideoProcessingFailedHandler(
        IVideoRepository repo,
        IMediator mediator) : IRequestHandler<VideoProcessingFailedEvent>
    {
        public async Task<Unit> Handle(VideoProcessingFailedEvent notification, CancellationToken cancellationToken)
        {
            var video = await repo.GetByIdAsync(notification.VideoId);

            if (video is null)
                return Unit.Value;

            video.GetType().GetProperty("Status")?.SetValue(video, Domain.Enums.Status.Failed);

            await mediator.Publish(new SendEmailEvent
            (
                "",
                video.FileName ?? "",
                Domain.Enums.Status.Failed,
                ""
            ), cancellationToken);

            return Unit.Value;
        }

    }
}
