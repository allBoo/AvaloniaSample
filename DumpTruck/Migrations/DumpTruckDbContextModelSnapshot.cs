﻿// <auto-generated />
using DumpTruck.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DumpTruck.Migrations
{
    [DbContext(typeof(DumpTruckDbContext))]
    partial class DumpTruckDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("DumpTruck.Models.DumpTruck", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("BodyColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("GarageId")
                        .HasColumnType("integer");

                    b.Property<int>("Speed")
                        .HasColumnType("integer");

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DumpTrucks");

                    b.HasDiscriminator<string>("Discriminator").HasValue("DumpTruck");
                });

            modelBuilder.Entity("DumpTruck.Models.Garage<DumpTruck.Models.IVehicle>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Garages");
                });

            modelBuilder.Entity("DumpTruck.Models.TipTruck", b =>
                {
                    b.HasBaseType("DumpTruck.Models.DumpTruck");

                    b.Property<bool>("Tent")
                        .HasColumnType("boolean");

                    b.Property<string>("TentColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Tipper")
                        .HasColumnType("boolean");

                    b.Property<string>("TipperColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("TipTruck");
                });
#pragma warning restore 612, 618
        }
    }
}
