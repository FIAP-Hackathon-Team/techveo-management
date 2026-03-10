using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Incoming;

public record VideoProcessingCompletedEvent(
    Guid VideoId,
    DateTime CompletedAt,
    string ZipKey)
    : IIntegrationEvent;
