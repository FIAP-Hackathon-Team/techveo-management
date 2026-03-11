using Microsoft.AspNetCore.Builder;

namespace TechVeo.Management.Infra
{
    public static class RequestPipeline
    {
        public static IApplicationBuilder UseInfra(this IApplicationBuilder app)
        {
            app.UseSharedInfra();

            return app;
        }
    }
}
