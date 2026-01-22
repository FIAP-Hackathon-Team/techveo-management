using Microsoft.AspNetCore.Http;

namespace TechVeo.Management.Contracts.Managements;

public record UploadVideoRequest(IFormFile file, int? snapshotCount, 
    double? intervalSeconds, int width, int height)
{
    public record Item(Guid ProductId, int Quantity);
}
