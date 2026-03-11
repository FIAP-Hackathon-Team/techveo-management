using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TechVeo.Management.Infra.Persistence.Contexts;
using TechVeo.Shared.Application.Storage;
using TechVeo.Shared.Domain.UoW;
using TechVeo.Shared.Infra.Extensions;
using TechVeo.Shared.Infra.Persistence.UoW;

namespace TechVeo.Management.Integration.Tests.Fixtures;

public class IntegrationTestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    public VideoContext DbContext { get; }

    public IntegrationTestFixture()
    {
        var services = new ServiceCollection();

        // Configure InfraOptions for VideoContext
        services.AddOptions<InfraOptions>();

        // Configure in-memory database
        services.AddDbContext<VideoContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        // Configure Unit of Work
        services.TryAddScoped<IUnitOfWorkTransaction, UnitOfWorkTransaction>();
        services.TryAddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<VideoContext>());

        // Register application services
        services.AddMediatR(typeof(Application.Commands.Video.Upload.UploadVideoCommand).Assembly);

        // Register domain repositories
        services.AddScoped<Domain.Repositories.IVideoRepository, Infra.Persistence.Repositories.VideoRepository>();

        // Mock external services
        var videoStorageServiceMock = new Mock<IVideoStorage>();
        services.AddScoped(_ => videoStorageServiceMock.Object);

        ServiceProvider = services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<VideoContext>();
    }

    public void Dispose()
    {
        DbContext?.Database.EnsureDeleted();
        DbContext?.Dispose();

        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
