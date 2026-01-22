using TechVeo.Management.Application;
using TechVeo.Management.Infra;
using TechVeo.Management.Infra.Persistence.Contexts;
using TechVeo.Shared.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddPresentation(builder.Configuration, new PresentationOptions
    {
        AddSwagger = true,
        AddJwtAuthentication = true,
        SwaggerTitle = "TechVeo Management API V1",
        SwaggerDescription = "TechVeo Management API V1"
    });

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

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger(options =>
        {
            options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
        });
    }

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
