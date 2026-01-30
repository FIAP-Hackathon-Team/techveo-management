using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Events.Integration.Incoming.Handlers
{
    internal class VideoProcessingFailedHandler(IVideoRepository repo)
    {
        public async Task Handle(VideoProcessingFailedEvent notification, CancellationToken cancellationToken)
        {
            var video = await repo.GetByIdAsync(notification.VideoId);
            if (video is null)
                return;

            video.GetType().GetProperty("Status")?.SetValue(video, Domain.Enums.Status.Failed);
        }
    }
}
