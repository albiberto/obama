using System.Reflection;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Obama;
using Obama.Domain;
using Serilog;

var name = Assembly.GetExecutingAssembly().GetName().Name;
var model = GetEdmModel();

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Information("{solution} building up...", name);

builder.AddLogger();
builder.AddOData(model);
builder.AddInfrastructure();

Log.Information("{solution} starting up...", name);

var app = builder.Build();

app.UseODataBatching(); 
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

Log.Information("{solution} started!", name);

await app.RunAsync();
return;

static IEdmModel GetEdmModel() 
{ 
    var builder = new ODataConventionModelBuilder(); 
            
    builder.EntitySet<Role>("Roles"); 
    builder.EntitySet<Employee>("Employees"); 
    
    builder.EntityType<Employee>()
        .Collection
        .Function("mostRecent")
        .Returns<string>();
        
    return builder.GetEdmModel(); 
}