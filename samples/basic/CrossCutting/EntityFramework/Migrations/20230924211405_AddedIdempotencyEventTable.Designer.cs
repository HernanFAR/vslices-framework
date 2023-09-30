﻿// <auto-generated />
using System;
using CrossCutting.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CrossCutting.EntityFramework.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230924211405_AddedIdempotencyEventTable")]
    partial class AddedIdempotencyEventTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("Domain.Entities.IdempotencyEvent", b =>
                {
                    b.Property<Guid>("EventId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("HandlerId")
                        .HasColumnType("TEXT");

                    b.HasKey("EventId", "HandlerId");

                    b.ToTable("IdempotencyEvents", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Question", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Questions", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
