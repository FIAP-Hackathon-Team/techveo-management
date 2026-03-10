using MediatR;
using Microsoft.AspNetCore.Http;
using TechVeo.Management.Application.Dto;

namespace TechVeo.Management.Application.Commands.Video.Upload;

public record UploadVideoCommand(
        Guid UserId,
        IFormFile File,
        int? SnapshotCount,
        double? IntervalSeconds,
        int Width,
        int Height) : IRequest<VideoDto>
{ }
