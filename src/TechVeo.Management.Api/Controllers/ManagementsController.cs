using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechVeo.Management.Application.Commands.Video.Query;
using TechVeo.Management.Application.Commands.Video.Upload;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Contracts.Managements;
using TechVeo.Shared.Application.Extensions;

namespace TechVeo.Management.Api.Controllers;

[ApiController()]
[Route("v1/[controller]")]
[Authorize]
public class ManagementsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<VideoDto>> CreateAsync([FromForm] UploadVideoRequest videos)
    {
        var userId = User.GetUserId()!;

        var result = await _mediator.Send(
                            new UploadVideoCommand(
                                userId,
                                videos.File,
                                videos.SnapshotCount,
                                videos.IntervalSeconds,
                                videos.Width,
                                videos.Height));

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<VideoDto>>> GetAllAsync()
    {
        var userId = User.GetUserId()!;

        var result = await _mediator.Send(new GetAllVideosByUserIdCommand(userId));

        return Ok(result);
    }
}
