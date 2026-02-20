using MediatR;
using TechVeo.Shared.Application.Events;

namespace TechVeo.Management.Application.Events.Integration.Incoming;

public record VideoProcessingFailedEvent(
    Guid VideoId,
    DateTime FailedAt)
    : IIntegrationEvent, IRequest;
