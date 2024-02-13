using FluentMigrator.Runner;

namespace Customers.Api;

public static class CustomersDbMigrator
{
    public static void Run(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }
}
