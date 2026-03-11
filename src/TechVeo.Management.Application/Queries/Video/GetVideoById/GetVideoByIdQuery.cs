using MediatR;
using TechVeo.Management.Application.Dto;

namespace TechVeo.Management.Application.Queries.Video.GetVideoById;

public record GetVideoByIdQuery(Guid UserId, Guid VideoId) : IRequest<VideoDto?>;
