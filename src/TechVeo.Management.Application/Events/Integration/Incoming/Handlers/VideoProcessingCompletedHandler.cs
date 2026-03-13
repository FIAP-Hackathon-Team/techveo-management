using MediatR;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Application.Services.Interfaces;
using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Shared.Application.Storage;

namespace TechVeo.Management.Application.Events.Integration.Incoming.Handlers
{
    internal class VideoProcessingCompletedHandler(
        IMediator mediator,
        IVideoRepository repo,
        IVideoStorage videoStorage,
        IAuthenticationService authService
    ) : INotificationHandler<VideoProcessingCompletedEvent>
    {
        private const int VideoUrlExpiresInHours = 24;

        public async Task Handle(VideoProcessingCompletedEvent notification, CancellationToken cancellationToken)
        {
            var video = await repo.GetByIdAsync(notification.VideoId);
            if (video is null)
            {
                return;
            }

            video.SetStatus(Status.Completed);

            var user = await authService.GetUserBydIdAsync(video.UserId, cancellationToken);
            var zipUrl = await videoStorage.GetVideoDownloadUrlAsync(notification.ZipKey!, TimeSpan.FromHours(VideoUrlExpiresInHours), cancellationToken);

            await mediator.Publish(new SendEmailEvent(
                user.Email!,
                video.FileName!,
                Status.Completed,
                zipUrl), cancellationToken);
        }
    }
}
