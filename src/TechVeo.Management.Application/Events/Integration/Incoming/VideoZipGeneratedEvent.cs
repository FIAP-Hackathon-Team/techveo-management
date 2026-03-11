using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Incoming;

public record VideoZipGeneratedEvent(
    Guid VideoId,
    string ZipKey
    ) : IIntegrationEvent;
