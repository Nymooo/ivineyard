using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Entities.Bookingobjects;
using Model.Entities.Bookingobjects.Machine;
using Model.Entities.Bookingobjects.Vineyard;
using Model.Entities.Company;
using Model.Entities.Harvest;

namespace Model.Configurations;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Machine> Machines { get; set; }
    public DbSet<MachineHasStatus> MachineHasStatus { get; set; }
    public DbSet<MachineStatusType> MachineStatusTypes { get; set; }
    public DbSet<Vineyard> Vineyards { get; set; }
    public DbSet<VineyardHasStatus> VineyardHasStati { get; set; }
    public DbSet<VineyardStatusType> VineyardStatusTypes { get; set; }
    public DbSet<WorkInformation> WorkInformation { get; set; }
    public DbSet<BookingObject> BookingObjects { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    //public DbSet<Measurement> Measurements { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Batch> Batches { get; set; }
    public DbSet<Informations> Informations { get; set; }
    public DbSet<StartingMust> StartingMusts { get; set; }
    public DbSet<Tank> Tanks { get; set; }
    public DbSet<TankHasWineBatch> TankHasWineBatches { get; set; }
    public DbSet<TankMovement> TankMovements { get; set; }
    public DbSet<Treatment> Treatments { get; set; }
    public DbSet<VineyardHasBatch> VineyardHasBatches { get; set; }
    public DbSet<WhiteWine_RedWine> WhiteWineRedWines { get; set; }
    public DbSet<WineBatchHasTreatment> WineBatchHasTreatments { get; set; }
    public DbSet<YoungWine> YoungWines { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        #region CONVERSIONS (EnumTypes)

        builder.Entity<MachineStatusType>()
            .Property(mst => mst.Type)
            .HasConversion<string>();

        builder.Entity<VineyardStatusType>()
            .Property(vhs => vhs.Type)
            .HasConversion<string>();
        
        builder.Entity<Treatment>()
            .Property(t => t.Type)
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

        #region VineyardHasWineBatch

        builder.Entity<VineyardHasBatch>()
            .HasOne(vhb => vhb.Vineyard)
            .WithMany()
            .HasForeignKey(vhb => vhb.VineyardId);
        
        builder.Entity<VineyardHasBatch>()
            .HasOne(vhb => vhb.Batch)
            .WithMany()
            .HasForeignKey(vhb => vhb.BatchId);
        
        #endregion
        
        #region TankHasWineBatch
        
        builder.Entity<TankHasWineBatch>()
            .HasOne(vhb => vhb.Batch)
            .WithMany()
            .HasForeignKey(vhb => vhb.BatchId);
        
        builder.Entity<TankHasWineBatch>()
            .HasOne(vhb => vhb.Tank)
            .WithMany()
            .HasForeignKey(vhb => vhb.TankId);
        
        #endregion
        
        #region WineBatchHasTreatment
        
        builder.Entity<WineBatchHasTreatment>()
            .HasOne(vhb => vhb.Batch)
            .WithMany(b => b.batchHasTreatmentsList)
            .HasForeignKey(vhb => vhb.BatchId);
        
        builder.Entity<WineBatchHasTreatment>()
            .HasOne(vhb => vhb.Treatment)
            .WithMany()
            .HasForeignKey(vhb => vhb.TreatementId);
        
        #endregion
        
        #region Information
        
        builder.Entity<Informations>()
            .HasOne(vhb => vhb.Batch)
            .WithMany(b => b.InformationsList)
            .HasForeignKey(vhb => vhb.BatchId);
        
        #endregion
        
        #region TankMovement
        
        builder.Entity<TankMovement>()
            .HasOne(vhb => vhb.FromTank)
            .WithMany()
            .HasForeignKey(vhb => vhb.FromTakId);
        
        builder.Entity<TankMovement>()
            .HasOne(vhb => vhb.ToTank)
            .WithMany()
            .HasForeignKey(vhb => vhb.ToTankId);
        
        #endregion

        #region StartingMust

        builder.Entity<StartingMust>()
            .HasOne(b => b.Informations)
            .WithOne(u => u.StartingMust)
            .HasForeignKey<StartingMust>(u => u.Id);

        #endregion

        #region YoungWine

        builder.Entity<YoungWine>()
            .HasOne(b => b.Informations)
            .WithOne(u => u.YoungWine)
            .HasForeignKey<YoungWine>(u => u.Id);

        #endregion

        #region WhiteWineRedWine

        builder.Entity<WhiteWine_RedWine>()
            .HasOne(b => b.Informations)
            .WithOne(u => u.WhiteWineRedWine)
            .HasForeignKey<WhiteWine_RedWine>(u => u.Id);

        #endregion
        
        #region KEYS

        builder.Entity<VineyardHasStatus>()
            .HasKey(vhs => new { vhs.VineyardId, vhs.StatusId, vhs.StartDate });

        builder.Entity<MachineHasStatus>()
            .HasKey(mhs => new { mhs.MachineId, mhs.StatusId, mhs.StartDate });

        builder.Entity<WorkInformation>()
            .HasKey(wi => new { wi.Id, wi.VineyardId, wi.UserId, wi.StartedAt });
        
        builder.Entity<TankHasWineBatch>()
            .HasKey(thwb => new { thwb.BatchId, thwb.TankId });
        
        builder.Entity<VineyardHasBatch>()
            .HasKey(vhb => new { vhb.BatchId, vhb.VineyardId });
        
        builder.Entity<WineBatchHasTreatment>()
            .HasKey(wbht => new { wbht.BatchId, wbht.TreatementId });

        #endregion
        
        #endregion
        
        base.OnModelCreating(builder);
    }
}