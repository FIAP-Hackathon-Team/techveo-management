using Microsoft.AspNetCore.Builder;
using TechVeo.Management.Application;
using TechVeo.Management.Infra;

var builder = Host.CreateApplicationBuilder(args);
{
    builder.Services.AddWorker();
    builder.Services.AddApplication();
    builder.Services.AddInfra();
}

var app = builder.Build();
{
    app.Run();
}
