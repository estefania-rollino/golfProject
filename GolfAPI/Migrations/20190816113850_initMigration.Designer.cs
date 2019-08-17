﻿// <auto-generated />
using System;
using GolfAPI.DataLayer.ADL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GolfAPI.Migrations
{
    [DbContext(typeof(GolfDatabaseContext))]
    [Migration("20190816113850_initMigration")]
    partial class initMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GolfAPI.DataLayer.DataModels.Component", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ComponentCode");

                    b.Property<string>("Description");

                    b.HasKey("Id");

                    b.ToTable("Components");
                });

            modelBuilder.Entity("GolfAPI.DataLayer.DataModels.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateRequired");

                    b.Property<string>("Description");

                    b.Property<string>("OrderNumber");

                    b.Property<int>("UserForeignKey");

                    b.HasKey("Id");

                    b.HasIndex("UserForeignKey");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("GolfAPI.DataLayer.DataModels.OrderComponent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ComponentId");

                    b.Property<int>("ComponentQuantity");

                    b.Property<int>("OrderId");

                    b.HasKey("Id");

                    b.HasIndex("ComponentId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrdersComponent");
                });

            modelBuilder.Entity("GolfAPI.DataLayer.DataModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Firstname");

                    b.Property<Guid>("Guid");

                    b.Property<string>("Lastname");

                    b.Property<string>("Password");

                    b.Property<int>("Role");

                    b.Property<string>("Username")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(16);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GolfAPI.DataLayer.DataModels.Order", b =>
                {
                    b.HasOne("GolfAPI.DataLayer.DataModels.User", "CreatedBy")
                        .WithMany("Orders")
                        .HasForeignKey("UserForeignKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GolfAPI.DataLayer.DataModels.OrderComponent", b =>
                {
                    b.HasOne("GolfAPI.DataLayer.DataModels.Component", "Component")
                        .WithMany("Orders")
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GolfAPI.DataLayer.DataModels.Order", "Order")
                        .WithMany("Components")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}