using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Entities.Bookingobjects;
using Model.Entities.Bookingobjects.Machine;
using Model.Entities.Bookingobjects.Vineyard;
using Model.Entities.Company;

namespace Model.Configurations;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        #region CONVERSIONS (EnumTypes)

        builder.Entity<MachineStatusType>()
            .Property(mst => mst.Type)
            .HasConversion<string>();

        builder.Entity<VineyardStatusType>()
            .Property(vhs => vhs.Type)
            .HasConversion<string>();

        #endregion

        #region UNIQUE CONSTRAINTS

        builder.Entity<Company>()
            .HasIndex(c => c.Name)
            .IsUnique();

        #endregion

        #region RELATIONS

        #region Booking Objects

        builder.Entity<BookingObject>()
            .HasOne(b => b.ApplicationUser)
            .WithOne(u => u.BookingObject)
            .HasForeignKey<ApplicationUser>(u => u.BookingObjectId);

        #endregion

        #region User

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Company)
            .WithMany()
            .HasForeignKey(u => u.CompanyId);

        #endregion

        #region WorkInformation

        builder.Entity<WorkInformation>()
            .HasOne(wi => wi.ApplicationUser)
            .WithMany()
            .HasForeignKey(wi => wi.UserId);

        builder.Entity<WorkInformation>()
            .HasOne(wi => wi.Vineyard)
            .WithMany()
            .HasForeignKey(wi => wi.VineyardId);

        builder.Entity<WorkInformation>()
            .HasOne(wi => wi.Machine)
            .WithMany(m => m.WorkInformation)
            .HasForeignKey(wi => wi.MachineId);

        #endregion

        #region Machines
        builder.Entity<MachineHasStatus>()
            .HasOne(mhs => mhs.Machine)
            .WithMany(m =>m.MachineHasStatusList)
            .HasForeignKey(mhs => mhs.MachineId);
           

        builder.Entity<MachineHasStatus>()
            .HasOne(mhs => mhs.MachineStatusType)
            .WithMany()
            .HasForeignKey(mhs => mhs.StatusId);

        #endregion

        #region Vineyards

        builder.Entity<Vineyard>()
            .HasOne(v => v.Company)
            .WithMany()
            .HasForeignKey(v => v.CompanyId);

        #endregion

        #region VineyardHasStatus

        builder.Entity<VineyardHasStatus>()
            .HasOne(vhs => vhs.Vineyard)
            .WithMany(v => v.StatusList)
            .HasForeignKey(vhs => vhs.VineyardId);

        builder.Entity<VineyardHasStatus>()
            .HasOne(vhs => vhs.VineyardStatusType)
            .WithMany(vs => vs.StatusList)
            .HasForeignKey(vhs => vhs.StatusId);

        #endregion

        #region Invoices

        builder.Entity<Invoice>()
            .HasOne(i => i.BookingObject)
            .WithMany(b =>b.Invoice)
            .HasForeignKey(i => i.BookingObjectId);

        #endregion

        #region KEYS

        builder.Entity<VineyardHasStatus>()
            .HasKey(vhs => new { vhs.VineyardId, vhs.StatusId, vhs.StartDate });

        builder.Entity<MachineHasStatus>()
            .HasKey(mhs => new { mhs.MachineId, mhs.StatusId, mhs.StartDate });

        builder.Entity<WorkInformation>()
            .HasKey(wi => new { wi.Id, wi.VineyardId, wi.UserId, wi.StartedAt });

        #endregion

        #endregion
        
        base.OnModelCreating(builder);
    }
}