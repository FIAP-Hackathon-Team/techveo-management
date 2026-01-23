using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Management.Infra.Persistence.Contexts;
using TechVeo.Management.Infra.Persistence.Repositories;
using TechVeo.Shared.Infra.Extensions;

namespace TechVeo.Management.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        services.AddSharedInfra<VideoContext>(new InfraOptions
        {
            DbContext = (serviceProvider, dbOptions) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                dbOptions.UseSqlServer(config.GetConnectionString("DataBaseConection"));
            },
            ApplicationAssembly = typeof(Application.DependencyInjection).Assembly
        });

        //Data
        services.AddScoped<IVideoRepository, VideoRepository>();

        //
        

        //Queries
        //services.AddScoped<IManagementQueryProvider, ManagementQueryProvider>();

        //Services

        return services;
    }
}
