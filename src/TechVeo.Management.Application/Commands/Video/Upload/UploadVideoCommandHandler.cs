using MediatR;
using Microsoft.AspNetCore.Http;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Shared.Application.Extensions;
using TechVeo.Shared.Application.Storage;

namespace TechVeo.Management.Application.Commands.Video.Upload;

public class UploadVideoCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IVideoRepository videoRepository,
    IVideoStorage videoStorage,
    IMediator mediator
        ) : IRequestHandler<UploadVideoCommand, VideoDto>
{
    public async Task<VideoDto> Handle(
        UploadVideoCommand request,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext!.User.GetUserId()!;

        var video = new Domain.Entities.Video(
            userId,
            request.snapshotCount,
            request.intervalSeconds,
            request.width,
            request.height);
      
        using (var stream = request.file.OpenReadStream())
        {
            var videoFileKey = await videoStorage.UploadVideoAsync(stream, request.file.FileName, cancellationToken);
            video.SetFileKey(videoFileKey);
        }

        await videoRepository.AddAsync(video);

        await mediator.Publish(
            new VideoUploadedEvent(
                video.Id,
                userId,
                video.FileKey!,
                video.CreateAt,
                new VideoUploadedMetadata(
                    video.Width,
                    video.Height,
                    video.SnapshotCount,
                    video.IntervalSeconds
                    )
                ), cancellationToken);

        return new VideoDto(video.Id, "",request.snapshotCount, request.width, request.height);
    }
}
