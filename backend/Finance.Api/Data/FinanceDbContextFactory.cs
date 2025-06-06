using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Finance.Api.Data;

public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), "app.env");
        DotNetEnv.Env.Load(envPath);

        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");

        optionsBuilder.UseNpgsql(connectionString);

        return new FinanceDbContext(optionsBuilder.Options);
    }
}