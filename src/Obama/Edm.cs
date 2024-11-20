using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Obama.Domain;

namespace Obama;

public static class Edm
{
    public static IEdmModel Build()
    {
        var builder = new ODataConventionModelBuilder();

        var employeeEntityType = builder.EntitySet<Employee>("Employees").EntityType;
        _ = builder.EntitySet<Role>("Roles").EntityType;

        employeeEntityType.HasRequired(e => e.Role);

        BuildUnboundFunctions(builder);
        BuildInboundFunctions(employeeEntityType);
        
        BuildUnboundActions(builder);
        BuildInboundActions(employeeEntityType);

        return builder.GetEdmModel();
    }

    private static void BuildUnboundFunctions(ODataConventionModelBuilder builder)
    {
        builder
            .Function("GetHighestBonus")
            .Returns<decimal>();        
    }

    private static void BuildInboundFunctions(EntityTypeConfiguration<Employee> configuration)
    {
        configuration
            .Function("GetBonus")
            .Returns<decimal>();

        configuration
            .Collection
            .Function("GetEmployeesByRole")
            .ReturnsCollectionFromEntitySet<Employee>("Employees")
            .Parameter<string>("role");
    }

    private static void BuildUnboundActions(ODataConventionModelBuilder builder)
    {
        builder
            .Action("ConferBonuses")
            .Parameter<decimal>("Bonus");
    }
    
    private static void BuildInboundActions(EntityTypeConfiguration<Employee> configuration)
    {
        configuration
            .Action("ConferBonus")
            .Parameter<decimal>("Bonus");
    }
}