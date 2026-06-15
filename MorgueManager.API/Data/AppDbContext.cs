using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MorgueManager.API.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Corpse> Corpses { get; set; }
    public DbSet<StorageSlot> StorageSlots { get; set; }
    public DbSet<TemperatureLog> TemperatureLogs { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public override int SaveChanges()
    {
        OnBeforeSaveChanges();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.Entity is TemperatureLog || entry.Entity is Notification || entry.Entity is Shift || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditLog
            {
                Timestamp = DateTime.Now,
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State.ToString().ToUpper()
            };

            var userEmail = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            auditEntry.UserEmail = string.IsNullOrEmpty(userEmail) ? "system@hospital.vn" : userEmail;

            var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            if (idProperty != null && idProperty.CurrentValue != null)
            {
                auditEntry.EntityId = idProperty.CurrentValue.ToString() ?? "";
            }

            var detailsList = new List<string>();
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsPrimaryKey()) continue;

                if (entry.State == EntityState.Added)
                {
                    detailsList.Add($"{property.Metadata.Name} = '{property.CurrentValue}'");
                }
                else if (entry.State == EntityState.Deleted)
                {
                    detailsList.Add($"{property.Metadata.Name} was deleted");
                }
                else if (entry.State == EntityState.Modified && property.IsModified)
                {
                    detailsList.Add($"{property.Metadata.Name}: '{property.OriginalValue}' -> '{property.CurrentValue}'");
                }
            }
            auditEntry.Details = string.Join("; ", detailsList);
            auditEntries.Add(auditEntry);
        }

        if (auditEntries.Any())
        {
            AuditLogs.AddRange(auditEntries);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Corpse>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            // Map NextOfKin as Owned Type (columns in the same table)
            entity.OwnsOne(c => c.NextOfKin);

            // Map AutopsyReport as Owned Type
            entity.OwnsOne(c => c.AutopsyReport);

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

        // Seed default Users (Access Key: MM-ACTIVE-2026)
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Email = "admin@hospital.vn", PasswordHash = "$2a$11$V6VVNz2xMBegNqXAB8HJNuBpYZFm9DClOl.fpQsvdB/up2G6TGK.W", DisplayName = "Admin User", Role = "Admin", CreatedAt = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = 2, Email = "manager@hospital.vn", PasswordHash = "$2a$11$V6VVNz2xMBegNqXAB8HJNuBpYZFm9DClOl.fpQsvdB/up2G6TGK.W", DisplayName = "Manager User", Role = "Manager", CreatedAt = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = 3, Email = "staff@hospital.vn", PasswordHash = "$2a$11$V6VVNz2xMBegNqXAB8HJNuBpYZFm9DClOl.fpQsvdB/up2G6TGK.W", DisplayName = "Staff User", Role = "Staff", CreatedAt = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed default Shifts for June 2026
        var shifts = new List<Shift>();
        int shiftId = 1;
        DateTime startDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        for (int d = 0; d < 30; d++)
        {
            DateTime date = startDate.AddDays(d);
            shifts.Add(new Shift { Id = shiftId++, StaffEmail = "staff@hospital.vn", Date = date, ShiftType = "Morning", Notes = "Ca sáng" });
            shifts.Add(new Shift { Id = shiftId++, StaffEmail = "manager@hospital.vn", Date = date, ShiftType = "Afternoon", Notes = "Ca chiều" });
            shifts.Add(new Shift { Id = shiftId++, StaffEmail = "staff@hospital.vn", Date = date, ShiftType = "Night", Notes = "Ca đêm" });
        }
        modelBuilder.Entity<Shift>().HasData(shifts);

        // Seed default StorageSlots A-01 to A-10, B-01 to B-10
        var slots = new List<StorageSlot>();
        int slotId = 1;
        for (int i = 1; i <= 10; i++)
        {
            slots.Add(new StorageSlot { Id = slotId++, SlotNumber = $"A-{i:D2}", UnitName = "Cold Room A", Status = SlotStatus.Empty, CurrentTemperature = 4.0 });
        }
        for (int i = 1; i <= 10; i++)
        {
            slots.Add(new StorageSlot { Id = slotId++, SlotNumber = $"B-{i:D2}", UnitName = "Cold Room B", Status = SlotStatus.Empty, CurrentTemperature = 4.0 });
        }
        modelBuilder.Entity<StorageSlot>().HasData(slots);
    }
}
