using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Incoming;

public record VideoZipGeneratedEvent(
    Guid VideoId,
    Guid ZipId,
    string ZipUrl
    ) : IIntegrationEvent;
