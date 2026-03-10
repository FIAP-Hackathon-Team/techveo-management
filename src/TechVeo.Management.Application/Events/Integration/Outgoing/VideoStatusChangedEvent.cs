using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Outgoing;

public record VideoStatusChangedEvent(
    Guid VideoId,
    string NewStatus,
    DateTime ChangedAt
) : IIntegrationEvent;
