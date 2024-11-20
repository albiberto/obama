using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Serilog;

namespace Obama;

public static class Bootstrapper
{
    public static void AddOData(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            // .AddOData(options => options
            //     .Select().Filter().OrderBy().Count().Expand()
            //     .AddRouteComponents("odata", Edm.Build(), new DefaultODataBatchHandler())
            // );
            .AddOData(options => options
                .EnableQueryFeatures(100)
                .AddRouteComponents("odata", Edm.Build(), new DefaultODataBatchHandler())
            );
    }

    public static void AddLogger(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(b => b.AddSerilog(Log.Logger, true));
    }
}