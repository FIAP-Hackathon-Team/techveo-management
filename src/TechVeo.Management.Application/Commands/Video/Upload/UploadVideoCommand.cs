using MediatR;
using Microsoft.AspNetCore.Http;
using TechVeo.Management.Application.Dto;

namespace TechVeo.Management.Application.Commands.Video.Upload;

public record UploadVideoCommand(IFormFile file,
        int? snapshotCount, double? intervalSeconds, int width, int height) : IRequest<VideoDto>
{ }
