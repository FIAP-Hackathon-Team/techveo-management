using System;

namespace TechVeo.Api.Application.Queries.GetById;

public class GetVideoByIdQuery : IRequest<VideoDto?>
{
    public GetVideoByIdQuery(Guid id)
        => Id = id;

    public Guid Id { get; set; }
}
