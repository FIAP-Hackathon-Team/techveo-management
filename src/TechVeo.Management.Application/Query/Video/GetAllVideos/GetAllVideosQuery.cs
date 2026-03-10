using MediatR;
using TechVeo.Management.Application.Dto;

namespace TechVeo.Management.Application.Query.Video.GetAllVideos;

public record GetAllVideosQuery(Guid UserId) : IRequest<List<VideoDto>>;
