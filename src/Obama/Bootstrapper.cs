using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.OData.Edm;
using Obama.Infrastructure;
using Obama.Infrastructure.DevSpace;
using Obama.Shared;
using Serilog;

namespace Obama;

public static class Bootstrapper
{
    public static void AddOData(this WebApplicationBuilder builder, IEdmModel edmModel)
    {
        builder.Services
            .AddControllers()
            .AddOData(options => options
                .EnableQueryFeatures(100)
                .AddRouteComponents("odata", edmModel, new DefaultODataBatchHandler())
            );
    }

    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

        if (!builder.Environment.IsProduction())
        {
            builder.Services.AddDevSeeder();
            builder.Services.AddMigration<ObamaContext, ObamaDevContextSeeder>();

            return;
        }

        builder.Services.AddMigration<ObamaContext>();
    }

    public static void AddLogger(this WebApplicationBuilder builder) => builder.Services.AddLogging(b => b.AddSerilog(Log.Logger, true));
}