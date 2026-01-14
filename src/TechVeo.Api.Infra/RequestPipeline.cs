using Microsoft.AspNetCore.Builder;

namespace TechVeo.Api.Infra
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
