using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Outgoing;

public record VideoUploadedEvent(
    Guid VideoId,
    Guid UserId,
    string VideoKey,
    DateTime UploadedAt,
    VideoUploadedMetadata Metadata
    ) : IIntegrationEvent;
