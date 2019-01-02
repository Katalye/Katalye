﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Katalye.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Katalye.Data.Migrations
{
    [DbContext(typeof(KatalyeContext))]
    [Migration("20190102025929_V1")]
    partial class V1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Katalye.Data.Entities.Job", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Arguments")
                        .IsRequired();

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<string>("Function")
                        .IsRequired();

                    b.Property<string>("Jid")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<List<string>>("MissingMinions");

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.Property<List<string>>("Target")
                        .IsRequired();

                    b.Property<string>("TargetType")
                        .IsRequired();

                    b.Property<DateTimeOffset>("TimeStamp");

                    b.Property<string>("User")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Jid")
                        .IsUnique();

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("Katalye.Data.Entities.JobMinion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<Guid?>("JobId")
                        .IsRequired();

                    b.Property<string>("MinionId")
                        .IsRequired();

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.HasKey("Id");

                    b.HasIndex("JobId", "MinionId")
                        .IsUnique();

                    b.ToTable("JobMinions");
                });

            modelBuilder.Entity("Katalye.Data.Entities.JobMinionReturnEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<Guid?>("JobMinionId")
                        .IsRequired();

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.Property<long>("ReturnCode");

                    b.Property<string>("ReturnData")
                        .IsRequired();

                    b.Property<bool>("Success");

                    b.Property<DateTimeOffset>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("JobMinionId");

                    b.ToTable("JobMinionEvents");
                });

            modelBuilder.Entity("Katalye.Data.Entities.Minion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<DateTimeOffset?>("LastAuthentication");

                    b.Property<string>("MinionSlug")
                        .IsRequired();

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("MinionSlug")
                        .IsUnique();

                    b.ToTable("Minions");
                });

            modelBuilder.Entity("Katalye.Data.Entities.MinionAuthenticationEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action")
                        .IsRequired();

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<Guid?>("MinionId")
                        .IsRequired();

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.Property<string>("PublicKey")
                        .IsRequired();

                    b.Property<bool>("Success");

                    b.Property<DateTimeOffset>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("MinionId");

                    b.ToTable("MinionAuthenticationEvents");
                });

            modelBuilder.Entity("Katalye.Data.Entities.UnknownEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<string>("Data")
                        .IsRequired();

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.Property<string>("Tag")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("UnknownEvents");
                });

            modelBuilder.Entity("Katalye.Data.Entities.JobMinion", b =>
                {
                    b.HasOne("Katalye.Data.Entities.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Katalye.Data.Entities.JobMinionReturnEvent", b =>
                {
                    b.HasOne("Katalye.Data.Entities.JobMinion", "JobMinion")
                        .WithMany()
                        .HasForeignKey("JobMinionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Katalye.Data.Entities.MinionAuthenticationEvent", b =>
                {
                    b.HasOne("Katalye.Data.Entities.Minion", "Minion")
                        .WithMany()
                        .HasForeignKey("MinionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
