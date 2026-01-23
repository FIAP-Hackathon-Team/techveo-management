using MediatR;
using TechVeo.Management.Application.Dto;

namespace TechVeo.Management.Application.Commands.Video.Query;

public record GetAllVideosByUserIdCommand(Guid UserId) : IRequest<List<VideoDto>>;
