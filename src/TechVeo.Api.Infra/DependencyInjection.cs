using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechVeo.Api.Infra.Persistence.Contexts;

namespace TechVeo.Api.Infra;

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
            ApplicationAssembly = typeof(DependencyInjection).Assembly
        });

        //Data

        return services;
    }
}
