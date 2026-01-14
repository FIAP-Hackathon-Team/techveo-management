using System.Threading;
using System.Threading.Tasks;

namespace TechVeo.Api.Application.Commands.SaveVideo;

public class SaveVideoCommandHandler(
        ) : IRequestHandler<SaveVideoCommand, SaveVideoCommandResponse>
{
    public async Task<SaveVideoCommandResponse> Handle(SaveVideoCommand request, CancellationToken cancellationToken)
    {
        throw new Exception();
    }
}
