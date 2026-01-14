using System.Threading;
using System.Threading.Tasks;

namespace TechVeo.Api.Application.Queries.GetById;

public class GetVideByIdQueryHandler() : IRequestHandler<GetVideoByIdQuery, GetVideoByIdQueryResponse>
{
    public async Task<VideoDto?> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
    {
        throw Exception();
    }
}
