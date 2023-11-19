﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VSlices.Core.DataAccess.EntityFramework.UnitTests.Contexts;

#nullable disable

namespace VSlices.Core.DataAccess.EntityFramework.UnitTests.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230522183451_Values")]
    partial class Values
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("VSlices.Core.DataAccess.EntityFramework.UnitTests.Contexts.AppDbContext+DbEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DbEntity");
                });

            modelBuilder.Entity("VSlices.Core.DataAccess.EntityFramework.UnitTests.Contexts.AppDbContext+Entity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Entity");
                });
#pragma warning restore 612, 618
        }
    }
}