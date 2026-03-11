using MediatR;
using TechVeo.Management.Application.Dto;

namespace TechVeo.Management.Application.Queries.Video.GetAllVideos;

public record GetAllVideosQuery(Guid UserId) : IRequest<List<VideoDto>>;
