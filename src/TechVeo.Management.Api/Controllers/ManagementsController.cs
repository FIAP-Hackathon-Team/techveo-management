using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechVeo.Management.Application.Commands.Video.Upload;
using TechVeo.Management.Contracts.Managements;


namespace TechVeo.Management.Api.Controllers;

[ApiController()]
[Route("v1/[controller]")]
[Authorize]
public class ManagementsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] UploadVideoRequest videos)
    {

        await _mediator.Publish(new UploadVideoCommand(videos.file, videos.snapshotCount, 
            videos.intervalSeconds, videos.width, videos.height));

        return Ok();
    }
}
