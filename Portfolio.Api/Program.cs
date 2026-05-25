using Microsoft.EntityFrameworkCore;
using Portfolio.Api;
using Portfolio.Api.Data;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PortfolioDb")
        ?? "Data Source=portfolio.db"));

builder.Services.AddScoped<ITickerRepository, TickerRepository>();
builder.Services.AddScoped<IHoldingRepository, HoldingRepository>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();

builder.Services.AddScoped<IHoldingsService, HoldingsService>();
builder.Services.AddScoped<IPricesService, PricesService>();

builder.Services.AddHostedService<PriceRefreshService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    await db.Database.MigrateAsync();
    await SeedRunner.SeedAsync(db);
}

app.Run();
