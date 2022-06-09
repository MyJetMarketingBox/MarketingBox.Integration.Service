﻿// <auto-generated />
using System;
using MarketingBox.Integration.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Integration.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20220520100521_Remove_Sequence")]
    partial class Remove_Sequence
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("integration-service")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MarketingBox.Integration.Postgres.Entities.RegistrationLogEntity", b =>
                {
                    b.Property<long>("RegistrationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("RegistrationId"));

                    b.Property<long>("AffiliateId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Crm")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CrmUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CustomerCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomerEmail")
                        .HasColumnType("text");

                    b.Property<string>("CustomerId")
                        .HasColumnType("text");

                    b.Property<DateTime>("DepositedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Depositor")
                        .HasColumnType("boolean");

                    b.Property<long>("IntegrationId")
                        .HasColumnType("bigint");

                    b.Property<string>("IntegrationName")
                        .HasColumnType("text");

                    b.Property<string>("RegistrationUniqueId")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("RegistrationId");

                    b.HasIndex("TenantId", "RegistrationId");

                    b.HasIndex("TenantId", "IntegrationId", "CustomerId");

                    b.ToTable("registrationslogs", "integration-service");
                });
#pragma warning restore 612, 618
        }
    }
}
