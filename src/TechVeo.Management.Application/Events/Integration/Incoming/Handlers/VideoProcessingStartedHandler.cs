using MediatR;
using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Events.Integration.Incoming.Handlers
{
    internal class VideoProcessingStartedHandler(IVideoRepository repo) : INotificationHandler<VideoProcessingStartedEvent>
    {
        public async Task Handle(VideoProcessingStartedEvent notification, CancellationToken cancellationToken)
        {
            var video = await repo.GetByIdAsync(notification.VideoId);
            if (video is null)
            {
                return;
            }

            video.SetStatus(Status.Processing);
        }
    }
}
