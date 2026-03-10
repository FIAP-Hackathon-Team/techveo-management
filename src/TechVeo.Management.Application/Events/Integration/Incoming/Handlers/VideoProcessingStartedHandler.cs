using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using System.Threading.Tasks;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Events.Integration.Incoming.Handlers
{
    internal class VideoProcessingStartedHandler(IVideoRepository repo, IMediator @object) : INotificationHandler<VideoProcessingStartedEvent>
    {
        public async Task Handle(VideoProcessingStartedEvent notification, CancellationToken cancellationToken)
        {
            var video = await repo.GetByIdAsync(notification.VideoId);
            if (video is null)
                return;

            video.GetType().GetProperty("Status")?.SetValue(video, Domain.Enums.Status.Processing);
        }
    }
}       
