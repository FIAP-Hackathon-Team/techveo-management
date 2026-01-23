using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Outgoing;

public record VideoSnapshotsGenerated(
       Guid VideoId,
       DateTime GeneratedAt
       ) : IIntegrationEvent;
