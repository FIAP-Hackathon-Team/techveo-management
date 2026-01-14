using TechFood.Order.Application;
using TechFood.Order.Infra;
using TechFood.Order.Infra.Persistence.Contexts;
using TechFood.Shared.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddPresentation(builder.Configuration, new PresentationOptions
    {
        AddSwagger = true,
        AddJwtAuthentication = true,
        SwaggerTitle = "TechVeo API V1",
        SwaggerDescription = "TechVeo API V1"
    });

    builder.Services.AddApplication();

    builder.Services.AddInfra();

    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("techveo.read", policy => policy.RequireClaim("scope", "techveo.read"))
        .AddPolicy("techveo.write", policy => policy.RequireClaim("scope", "techveo.write"));
}

var app = builder.Build();
{
    app.RunMigration<OrderContext>();

    app.UsePathBase("/api/techveo");

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
