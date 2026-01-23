using MediatR;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Commands.Video.Query;

public class GetAllVideosByUserIdCommandHandler(IVideoRepository videoRepository) : IRequestHandler<GetAllVideosByUserIdCommand, List<VideoDto>>
{
    public async Task<List<VideoDto>> Handle(
        GetAllVideosByUserIdCommand request,
        CancellationToken cancellationToken)
    {

        var result = await videoRepository.GetByUserIdAsync(request.UserId);

        return result.Select(video => new VideoDto(
            video.Id,
            video.Status,
            video.FileName,
            video.IntervalSeconds,
            video.SnapshotCount,
            video.Width,
            video.Height
        )).ToList();

    }
}
