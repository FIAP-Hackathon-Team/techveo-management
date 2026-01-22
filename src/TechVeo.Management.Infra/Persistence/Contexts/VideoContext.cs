using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TechVeo.Management.Domain.Entities;
using TechVeo.Shared.Infra.Extensions;
using TechVeo.Shared.Infra.Persistence.Contexts;

namespace TechVeo.Management.Infra.Persistence.Contexts;

public class VideoContext : TechVeoContext
{
    public DbSet<Video> Videos { get; set; } = null!;

    public VideoContext(
        IOptions<InfraOptions> infraOptions,
        DbContextOptions<VideoContext> options
            ) : base(infraOptions, options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VideoContext).Assembly);

        var properties = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties());

        var stringProperties = properties.Where(p => p.ClrType == typeof(string));
        foreach (var property in stringProperties)
        {
            var maxLength = property.GetMaxLength() ?? 50;

            property.SetColumnType($"varchar({maxLength})");
        }

        var booleanProperties = properties
            .Where(p => p.ClrType == typeof(bool) ||
                        p.ClrType == typeof(bool?));

        foreach (var property in booleanProperties)
        {
            property.SetColumnType("bit");
            property.IsNullable = false;
        }

        var dateTimeProperties = properties.Where(p => p.ClrType == typeof(DateTime));

        foreach (var property in dateTimeProperties)
        {
            property.SetColumnType("datetime");
        }

        var enumProperties = properties.Where(p => p.ClrType == typeof(Enum));

        foreach (var property in enumProperties)
        {
            property.SetColumnType("smallint");
        }

        var amountProperties = properties
            .Where(p => p.ClrType == typeof(decimal) ||
                        p.ClrType == typeof(decimal?));

        foreach (var property in amountProperties)
        {
            property.SetColumnType("decimal(6, 2)");
        }

        SeedContext(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void SeedContext(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Video>()
            .HasData(
                new
                {
                },
                new
                {
                }
            );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.LogTo(Console.WriteLine);
#endif
    }
}
