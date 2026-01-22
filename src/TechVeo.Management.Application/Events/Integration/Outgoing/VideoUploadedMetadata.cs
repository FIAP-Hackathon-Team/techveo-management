namespace TechVeo.Management.Application.Events.Integration.Outgoing;

public record VideoUploadedMetadata(
int Width,
int Height,
int? SnapshotCount,
double? IntervalSeconds);