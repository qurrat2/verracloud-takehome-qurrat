using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

    public DbSet<Ticker> Tickers => Set<Ticker>();
    public DbSet<Holding> Holdings => Set<Holding>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Ticker>(b =>
        {
            b.HasKey(t => t.Id);

            b.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(16);

            b.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(128);

            b.Property(t => t.Description)
                .HasMaxLength(512);

            b.Property(t => t.CurrentPrice)
                .HasPrecision(18, 4);

            b.Property(t => t.LastUpdatedAt)
                .IsRequired();

            b.HasIndex(t => t.Code)
                .IsUnique();
        });

        builder.Entity<Holding>(b =>
        {
            b.HasKey(h => h.Id);

            b.Property(h => h.Quantity)
                .HasPrecision(18, 4);

            b.Property(h => h.PurchasePrice)
                .HasPrecision(18, 4);

            b.Property(h => h.CreatedAt)
                .IsRequired();

            // Allow multiple holdings to  reference the same ticker; do not enforce uniqueness here.
            b.HasIndex(h => h.TickerId);

            b.HasOne(h => h.Ticker)
                .WithMany()
                .HasForeignKey(h => h.TickerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}