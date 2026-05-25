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
const string DevCorsPolicy = "AllowDevFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(DevCorsPolicy, policy =>
    {
        // Dev only: allow the frontend on any localhost port, since Vite falls
        // back to 5174+ when 5173 is taken.
        policy.SetIsOriginAllowed(origin =>
            {
                var host = new Uri(origin).Host;
                return host == "localhost" || host == "127.0.0.1";
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors(DevCorsPolicy);
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    await db.Database.MigrateAsync();
    // WAL lets the polling reads run concurrently with the background refresh
    // writes instead of blocking on SQLite's default journal mode.
    await db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");
    await SeedRunner.SeedAsync(db);
}

app.Run();
