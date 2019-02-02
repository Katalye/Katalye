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
        public DbSet<ServerSetting> ServerSettings { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<MinionReturnEvent> MinionReturnEvents { get; set; }

        public DbSet<JobCreationEvent> JobCreationEvents { get; set; }

        public DbSet<UnknownEvent> UnknownEvents { get; set; }

        public DbSet<Minion> Minions { get; set; }

        public DbSet<MinionAuthenticationEvent> MinionAuthenticationEvents { get; set; }

        public DbSet<MinionGrain> MinionGrains { get; set; }

        public DbSet<MinionGrainValue> MinionGrainValues { get; set; }

        public KatalyeContext(DbContextOptions<KatalyeContext> options) : base(options)
        {
        }

        public KatalyeContext(string connectionString) : this(new DbContextOptionsBuilder<KatalyeContext>()
                                                              .UseNpgsql(connectionString).Options)
        {
            // Hard-coded for LinqPad.
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            // Job
            model.Entity<Job>()
                 .HasIndex(p => new {p.Jid})
                 .IsUnique();
            model.Entity<Job>()
                 .Property(x => x.Arguments)
                 .IsRequired()
                 .HasConversion(
                     list => list.ToString(),
                     s => ParseJson(s) as JArray
                 );

            // MinionReturnEvent
            model.Entity<MinionReturnEvent>()
                 .Property(x => x.ReturnData)
                 .IsRequired()
                 .HasConversion(
                     obj => obj.ToString(),
                     s => ParseJson(s)
                 );

            // UnknownEvent
            model.Entity<UnknownEvent>()
                 .Property(x => x.Data)
                 .IsRequired()
                 .HasConversion(
                     obj => obj.ToString(),
                     s => ParseJson(s)
                 );

            // Minion
            model.Entity<Minion>()
                 .HasIndex(x => x.MinionSlug)
                 .IsUnique();
            model.Entity<Minion>()
                 .HasIndex(x => x.GrainGeneration)
                 .IsUnique();

            // MinionReturnEvent
            model.Entity<MinionReturnEvent>()
                 .HasIndex(
                     nameof(MinionReturnEvent.MinionId),
                     nameof(MinionReturnEvent.JobId)
                 )
                 .IsUnique();

            // JobCreationEvent
            model.Entity<JobCreationEvent>()
                 .HasIndex(x => x.JobId)
                 .IsUnique();

            // MinionAuthenticationEvent
            model.Entity<MinionAuthenticationEvent>()
                 .HasIndex(x => x.PublicKeyHash);

            // MinionGrain
            model.Entity<MinionGrain>()
                 .HasIndex(
                     nameof(MinionGrain.MinionId),
                     nameof(MinionGrain.Generation),
                     nameof(MinionGrain.Path)
                 )
                 .IsUnique();
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

            foreach (var entity in ChangeTracker.Entries<IConcurrencyCheck>())
            {
                entity.Entity.Version += 1;
            }
        }

        private JToken ParseJson(string json)
        {
            if (json == "True")
            {
                return JToken.FromObject(true);
            }

            if (json == "False")
            {
                return JToken.FromObject(false);
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return JToken.FromObject("");
            }

            if (json.StartsWith("[") || json.StartsWith("{"))
            {
                return JToken.Parse(json);
            }

            return JToken.FromObject(json);
        }
    }
}