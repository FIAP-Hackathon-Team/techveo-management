using MediatR;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Domain.Repositories;

namespace TechVeo.Management.Application.Queries.Video.GetVideoById;

public class GetVideoByIdQueryHandler(IVideoRepository videoRepository) : IRequestHandler<GetVideoByIdQuery, VideoDto?>
{
    public async Task<VideoDto?> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await videoRepository.GetByIdAsync(request.UserId, request.VideoId);

        return result is null
            ? null
            : new VideoDto(
                result.Id,
                result.Status,
                result.FileName,
                result.IntervalSeconds,
                result.SnapshotCount,
                result.Width,
                result.Height);
    }
}
