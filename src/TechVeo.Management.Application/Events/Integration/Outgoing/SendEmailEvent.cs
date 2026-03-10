using TechVeo.Management.Domain.Enums;
using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Outgoing
{
    public record SendEmailEvent(string EmailAddress,
        string FileName,
        Status Status,
        string Url) : IIntegrationEvent;
}
