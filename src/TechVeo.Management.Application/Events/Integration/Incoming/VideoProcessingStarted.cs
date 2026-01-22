using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Incoming;

public record VideoProcessingStarted(
    Guid VideoId,
    DateTime StartedAt
    ) : IIntegrationEvent;
