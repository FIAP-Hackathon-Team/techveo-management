using MediatR;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Events.Integration.Incoming.Handlers
{
    internal class VideoProcessingCompletedHandler(IVideoRepository repo, IMediator mediator) : INotificationHandler<VideoProcessingCompletedEvent>
    {
        public async Task Handle(VideoProcessingCompletedEvent notification, CancellationToken cancellationToken)
        {
            var video = await repo.GetByIdAsync(notification.VideoId);
            if (video is null)
                return;

            video.GetType().GetProperty("Status")?.SetValue(video, Domain.Enums.Status.Completed);

            //await mediator.Publish(new SendEmailEvent(
            //        user.E
            //    ));
        }
    }
}
