using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Models;

namespace MorgueManager.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Corpse> Corpses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Corpse>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            // Map NextOfKin as Owned Type (columns in the same table)
            entity.OwnsOne(c => c.NextOfKin);

            // Map collections as Owned Types
            entity.OwnsMany(c => c.Belongings, b =>
            {
                b.WithOwner().HasForeignKey("CorpseId");
                b.Property<int>("Id");
                b.HasKey("Id");
            });

            entity.OwnsMany(c => c.History, h =>
            {
                h.WithOwner().HasForeignKey("CorpseId");
                h.Property<int>("Id");
                h.HasKey("Id");
            });

            entity.OwnsMany(c => c.Documents, d =>
            {
                d.WithOwner().HasForeignKey("CorpseId");
                d.Property<int>("Id");
                d.HasKey("Id");
            });
        });
    }
}
