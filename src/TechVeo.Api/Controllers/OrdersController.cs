using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechVeo.Api.Application.Commands.SaveVideo;
using TechVeo.Api.Contracts.Video;

namespace TechVeo.Api.Controllers;

[ApiController()]
[Route("v1/[controller]")]
[Authorize]
public class VideoController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Authorize(Policy = "video.write")]
    public async Task<IActionResult> CreateAsync(SaveVideoRequest request)
    {

        var result = await _mediator.Send(new SaveVideoCommand());

        return Ok(result);
    }
}
