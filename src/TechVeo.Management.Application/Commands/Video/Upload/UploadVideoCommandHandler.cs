using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Shared.Application.Extensions;
using TechVeo.Shared.Application.Storage;

namespace TechVeo.Management.Application.Commands.Video.Upload;

public class UploadVideoCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IVideoRepository videoRepository,
    IVideoStorage videoStorage
        ) : IRequestHandler<UploadVideoCommand, VideoDto>
{
    public async Task<VideoDto> Handle(UploadVideoCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.GetUserId();

        var video = new Domain.Entities.Video(Guid.NewGuid(), request.snapshotCount,
            request.intervalSeconds, request.width, request.height);
       
        await videoRepository.AddAsync(video);

        using (var stream = request.file.OpenReadStream())
            await videoStorage.UploadVideoAsync(stream, request.file.FileName, cancellationToken);

        return new VideoDto(video.Id, "",request.snapshotCount, request.width, request.height);
    }
}
