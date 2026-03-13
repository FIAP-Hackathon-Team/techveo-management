using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TechVeo.Management.Application;
using TechVeo.Management.Infra;
using TechVeo.Management.Infra.Persistence.Contexts;
using TechVeo.Shared.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    var maxBodySize = 1L * 1024 * 1024 * 1024; // 1 GB;

    builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = maxBodySize);
    builder.Services.Configure<IISServerOptions>(options => options.MaxRequestBodySize = maxBodySize);
    builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = maxBodySize);

    builder.Services.AddPresentation(builder.Configuration, new PresentationOptions
    {
        AddSwagger = true,
        AddJwtAuthentication = true,
        SwaggerTitle = "TechVeo Management API V1",
        SwaggerDescription = "TechVeo Management API V1"
    });

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService("techveo-management-api"))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(o =>
            {
                o.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"]!);
            }));

    builder.Services.AddApplication();

    builder.Services.AddInfra();

    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("managements.read", policy => policy.RequireClaim("scope", "managements.read"))
        .AddPolicy("managements.write", policy => policy.RequireClaim("scope", "managements.write"));
}

var app = builder.Build();
{
    app.RunMigration<VideoContext>();

    app.UsePathBase("/api/management");

    app.UseForwardedHeaders();

    // Allow large request bodies per-request (removes the per-request limit)
    app.Use(async (context, next) =>
    {
        var maxFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (maxFeature != null)
        {
            // null means unlimited. Set to a numeric value (bytes) to limit.
            maxFeature.MaxRequestBodySize = null;
        }

        await next();
    });

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger(options =>
    {
        options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
    });

    app.UseSwaggerUI();

    app.UseInfra();

    app.UseHealthChecks("/health");

    app.UseRouting();

    app.UseCors();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
