using MediatR;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Application.Events.Integration.Incoming;
using TechVeo.Management.Application.Events.Integration.Outgoing;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Shared.Application.Storage;

namespace TechVeo.Management.Application.Commands.Video.Upload;

public class GetAllVideosByUserIdCommandHandler(
    IVideoRepository videoRepository,
    IVideoStorage videoStorage,
    IMediator mediator
        ) : IRequestHandler<UploadVideoCommand, VideoDto>
{
    public async Task<VideoDto> Handle(
        UploadVideoCommand request,
        CancellationToken cancellationToken)
    {
        var video = new Domain.Entities.Video(
            request.UserId,
            request.File.FileName,
            request.SnapshotCount,
            request.IntervalSeconds,
            request.Width,
            request.Height);
      
        using (var stream = request.File.OpenReadStream())
        {
            var videoFileKey = await videoStorage.UploadVideoAsync(stream, request.File.FileName, cancellationToken);
            video.SetFileKey(videoFileKey);
        }

        await videoRepository.AddAsync(video);

        await mediator.Publish(
            new VideoUploadedEvent(
                video.Id,
                video.UserId,
                video.FileKey!,
                video.CreateAt,
                new VideoUploadedMetadata(
                    video.Width,
                    video.Height,
                    video.SnapshotCount,
                    video.IntervalSeconds
                    )
                ), cancellationToken);

        return new VideoDto(video.Id, video.Status, video.FileName, video.IntervalSeconds, video.SnapshotCount, video.Width, video.Height);
    }
}
