using MediatR;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Application.Services.Interfaces;
using TechVeo.Management.Domain.Enums;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Events.Integration.Incoming.Handlers
{
    internal class VideoProcessingCompletedHandler(
        IMediator mediator,
        IVideoRepository repo,
        IAuthenticationService authService
    ) : INotificationHandler<VideoProcessingCompletedEvent>
    {
        public async Task Handle(VideoProcessingCompletedEvent notification, CancellationToken cancellationToken)
        {
            var video = await repo.GetByIdAsync(notification.VideoId);

            if (video is null)
            {
                return;
            }

            video.SetStatus(Status.Completed);

            var user = await authService.GetUserBydIdAsync(video.UserId, cancellationToken);

            await mediator.Publish(new SendEmailEvent(
                user.Email!,
                video.FileName ?? "",
                Status.Completed,
                notification.Url), cancellationToken);
        }
    }
}
