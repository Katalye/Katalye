using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Katalye.Data.Entities;
using Katalye.Data.Interfaces;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

namespace Katalye.Data
{
    [UsedImplicitly]
    public class KatalyeContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobMinion> JobMinions { get; set; }

        public DbSet<JobMinionEvent> JobMinionEvents { get; set; }

        public KatalyeContext(DbContextOptions<KatalyeContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                        .HasIndex(p => new {p.Jid})
                        .IsUnique();

            modelBuilder.Entity<Job>()
                        .Property(x => x.Arguments)
                        .IsRequired()
                        .HasConversion(
                            list => list.ToString(),
                            s => JArray.Parse(s)
                        );

            modelBuilder.Entity<JobMinion>()
                        .HasIndex(p => new {p.JobId, p.MinionId})
                        .IsUnique();

            modelBuilder.Entity<JobMinionEvent>()
                        .Property(x => x.Type)
                        .IsRequired()
                        .HasConversion<string>();

            modelBuilder.Entity<JobMinionEvent>()
                        .Property(x => x.Data)
                        .IsRequired()
                        .HasConversion(
                            obj => obj.ToString(),
                            s => JObject.Parse(s)
                        );
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyConventions();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyConventions()
        {
            foreach (var entity in ChangeTracker.Entries<IEntity>())
            {
                entity.Entity.Id = entity.Entity.Id == default(Guid) ? RT.Comb.Provider.PostgreSql.Create() : entity.Entity.Id;
            }

            foreach (var entity in ChangeTracker.Entries<IAuditable>())
            {
                if (entity.State == EntityState.Modified)
                {
                    entity.Entity.ModifiedOn = DateTimeOffset.Now;
                }

                entity.Entity.CreatedOn = entity.Entity.CreatedOn == default(DateTimeOffset) ? DateTimeOffset.Now : entity.Entity.CreatedOn;
                entity.Entity.ModifiedOn = entity.Entity.ModifiedOn == default(DateTimeOffset) ? DateTimeOffset.Now : entity.Entity.ModifiedOn;
            }
        }
    }
}