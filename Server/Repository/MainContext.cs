using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using BCN.Services;
using BCN.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Repository.Databases.Interfaces;

namespace BCN.Server.Repository
{
    public class MainContext : DBContextBase, ITransientService, IMultiDatabase
    {
        private readonly ICurrentUserService _currentUserService;

        public MainContext(IDBContextDependencies dBContextDependencies, ICurrentUserService currentUserService)
            : base(dBContextDependencies)
        {
            _currentUserService = currentUserService;

            // ContextBase handles multi-tenant database connections
        }

        public virtual DbSet<BCNUser> Users { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Log> Logs { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BCNUser>().ToTable("User");
            builder.Entity<Role>().ToTable("Role");
            builder.Entity<Log>().ToTable("Log");
            builder.Entity<Notification>().ToTable("Notification");

            ConfigureUser(builder);
            ConfigureRole(builder);

            // ConfigureUserRole(builder);
            // ConfigureNotification(builder);
            ConfigureLog(builder);
        }

        private static void ConfigureUser(ModelBuilder builder)
        {
            builder.Entity<BCNUser>(entity =>
            {
                entity.ToTable("User");

                entity.HasKey(u => u.UserId);

                entity.Property(u => u.UserId);

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.Email)
                    .HasMaxLength(256);

                //entity.HasMany(u => u.UserRoles)
                //.WithOne().HasForeignKey(ur => ur.UserId);
            });
        }
        private static void ConfigureRole(ModelBuilder builder)
        {
            builder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.HasKey(r => r.RoleId);

                entity.HasIndex(r => new { r.SiteId, r.Name })
                    .IsUnique();

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });
        }

        private static void ConfigureLog(ModelBuilder builder)
        {
            builder.Entity<Log>(entity =>
            {
                entity.ToTable("Log");

                entity.HasKey(x => x.LogId);

                entity.Property(x => x.LogId)
                    .HasColumnName("LogId")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.SiteId)
                    .IsRequired();

                entity.Property(x => x.LogDate)
                    .IsRequired();

                entity.Property(x => x.PageId)
                    .IsRequired(false);

                entity.Property(x => x.ModuleId)
                    .IsRequired(false);

                entity.Property(x => x.UserId)
                    .IsRequired(false);

                entity.Property(x => x.Url)
                    .HasMaxLength(500);

                entity.Property(x => x.Server)
                    .HasMaxLength(200);

                entity.Property(x => x.Category)
                    .HasMaxLength(100);

                entity.Property(x => x.Feature)
                    .HasMaxLength(100);

                entity.Property(x => x.Function)
                    .HasMaxLength(100);

                entity.Property(x => x.Level)
                    .HasMaxLength(50);

                entity.Property(x => x.Message)
                    .HasColumnType("nvarchar(max)");

                entity.Property(x => x.MessageTemplate)
                    .HasColumnType("nvarchar(max)");

                entity.Property(x => x.Exception)
                    .HasColumnType("nvarchar(max)");

                entity.Property(x => x.Properties)
                    .HasColumnType("nvarchar(max)");
            });
        }

        private void ApplyAuditInfo()
        {
            var username = !string.IsNullOrWhiteSpace(_currentUserService.UserName) ? _currentUserService.UserName : "System";

            var entries = ChangeTracker
                .Entries<IAuditable>();

            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = now;
                    entry.Entity.CreatedBy = username;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedOn = now;
                    entry.Entity.ModifiedBy = username;
                }
            }

            var deletedEntries = ChangeTracker
                .Entries<IDeletable>();

            foreach (var deletedEntry in deletedEntries)
            {
                if (deletedEntry.Entity.IsDeleted)
                {
                    deletedEntry.Entity.DeletedOn = now;
                    deletedEntry.Entity.DeletedBy = username;
                }
            }
        }
    }
}
