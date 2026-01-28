using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Incoming;

public record VideoProcessingStartedEvent(
    Guid VideoId,
    DateTime StartedAt
    ) : IIntegrationEvent;
