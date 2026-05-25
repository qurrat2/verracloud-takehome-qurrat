using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portfolio.Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
{
    public PortfolioDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PortfolioDbContext>();

        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__PortfolioDb")
            ?? "Data Source=portfolio.db";

        builder.UseSqlite(connectionString);

        return new PortfolioDbContext(builder.Options);
    }
}
