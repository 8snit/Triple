using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Serilog;
using Serilog.Events;
using Triple.Api.Migration;

namespace Triple.Api.Model
{
    public class TripleContext : DbContext
    {
        public TripleContext()
            :
                base("TripleDB")
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = false;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TripleContext, TripleDbMigrationsConfiguration>());
        }

        public DbSet<Trip> Trips { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<Ride> Rides { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            foreach (var auditableEntity in this.ChangeTracker.Entries<IAuditable>())
            {
                if (auditableEntity.State == EntityState.Added)
                {
                    auditableEntity.Entity.CreatedAt = DateTimeOffset.Now;

                    if (Log.IsEnabled(LogEventLevel.Verbose))
                    {
                        Log.Verbose("creation of {@Entity}", auditableEntity);
                    }
                }
                else if (auditableEntity.State == EntityState.Modified)
                {
                    auditableEntity.Entity.LastChangedAt = DateTimeOffset.Now;
                    auditableEntity.Property(p => p.CreatedAt).IsModified = false;

                    if (Log.IsEnabled(LogEventLevel.Verbose))
                    {
                        Log.Verbose("update of {@Entity}", auditableEntity);
                    }
                }
                else if (auditableEntity.State == EntityState.Deleted)
                {
                    Log.Information("deletion of {@Entity}", auditableEntity);
                }
            }
            return base.SaveChanges();
        }
    }
}
