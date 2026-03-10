using MediatR;
using TechVeo.Management.Application.Dto;

namespace TechVeo.Management.Application.Queries.GetAllVideosByUserId;

public record GetAllVideosQuery(Guid UserId) : IRequest<List<VideoDto>>;
