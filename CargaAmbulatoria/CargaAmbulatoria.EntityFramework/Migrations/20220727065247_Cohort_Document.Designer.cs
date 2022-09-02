﻿// <auto-generated />
using System;
using CargaAmbulatoria.EntityFramework.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CargaAmbulatoria.EntityFramework.Migrations
{
    [DbContext(typeof(CargaAmbulatoriaDbContext))]
    [Migration("20220727065247_Cohort_Document")]
    partial class Cohort_Document
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CargaAmbulatoria.EntityFramework.Models.Cohort", b =>
                {
                    b.Property<long>("CohortId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("CohortId"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CohortId");

                    b.ToTable("Cohorts");

                    b.HasData(
                        new
                        {
                            CohortId = 1L,
                            Name = "ERC"
                        });
                });

            modelBuilder.Entity("CargaAmbulatoria.EntityFramework.Models.Document", b =>
                {
                    b.Property<long>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("DocumentId"), 1L, 1);

                    b.Property<long>("CohortId")
                        .HasColumnType("bigint");

                    b.Property<string>("Dni")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DocumentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Regime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("DocumentId");

                    b.HasIndex("CohortId");

                    b.HasIndex("UserId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("CargaAmbulatoria.EntityFramework.Models.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PasswordStored")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TokenReset")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("TokenResetExpiration")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = "302e8c06-302a-4e0b-9a19-d70a6541c655",
                            Email = "admin@coosalud.com",
                            Name = "Admin",
                            PasswordStored = "NhPGnizPnx4RQRSNWFXaRw==",
                            Role = 0,
                            Status = 0
                        },
                        new
                        {
                            UserId = "75ac2d91-4747-48d2-97c5-8d18e98fc58e",
                            Email = "agent@coosalud.com",
                            Name = "Agent",
                            PasswordStored = "NhPGnizPnx4RQRSNWFXaRw==",
                            Role = 1,
                            Status = 0
                        });
                });

            modelBuilder.Entity("CargaAmbulatoria.EntityFramework.Models.Document", b =>
                {
                    b.HasOne("CargaAmbulatoria.EntityFramework.Models.Cohort", "Cohort")
                        .WithMany()
                        .HasForeignKey("CohortId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargaAmbulatoria.EntityFramework.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cohort");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
