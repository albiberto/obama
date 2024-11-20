using System.Reflection;
using Microsoft.AspNetCore.OData;
using Obama;
using Obama.Infrastructure;
using Serilog;

var name = Assembly.GetExecutingAssembly().GetName().Name;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Information("{solution} building up...", name);

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddCors(options => options.AddDefaultPolicy(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.AddLogger();
builder.AddOData();

Log.Information("{solution} starting up...", name);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseODataBatching();
app.UseRouting();

if (builder.Environment.IsDevelopment()) app.UseODataRouteDebug();

app.MapControllers();

Log.Information("{solution} started!", name);

await app.RunAsync();