using System;
using System.Collections.Generic;

namespace TechVeo.Api.Application.Commands.SaveVideo;

public record SaveVideoCommand(Guid? CustomerId, List<SaveVideoCommand.Item> Items) : IRequest<SaveVideoCommandResponse>
{
    public record Item(Guid ProductId, int Quantity);
}
