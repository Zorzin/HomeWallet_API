﻿// <auto-generated />
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace HomeWallet_API.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20171109194211_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("HomeWallet_API.Models.Category", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("HomeWallet_API.Models.Plan", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<DateTime>("EndDate");

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("HomeWallet_API.Models.Product", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("HomeWallet_API.Models.ProductCategory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryID");

                    b.Property<int>("ProductID");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("ProductID");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("HomeWallet_API.Models.Receipt", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("PurchaseDate");

                    b.Property<int>("ShopID");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("ShopID");

                    b.HasIndex("UserID");

                    b.ToTable("Receipts");
                });

            modelBuilder.Entity("HomeWallet_API.Models.ReceiptProduct", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<double>("Price");

                    b.Property<int>("ProductID");

                    b.Property<int>("ReceiptID");

                    b.HasKey("ID");

                    b.HasIndex("ProductID");

                    b.HasIndex("ReceiptID");

                    b.ToTable("ReceiptProducts");
                });

            modelBuilder.Entity("HomeWallet_API.Models.Shop", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Shops");
                });

            modelBuilder.Entity("HomeWallet_API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Currency");

                    b.Property<string>("Email");

                    b.Property<bool>("Mode");

                    b.Property<string>("Nick");

                    b.Property<string>("Password");

                    b.Property<string>("Role");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HomeWallet_API.Models.Category", b =>
                {
                    b.HasOne("HomeWallet_API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HomeWallet_API.Models.Plan", b =>
                {
                    b.HasOne("HomeWallet_API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HomeWallet_API.Models.Product", b =>
                {
                    b.HasOne("HomeWallet_API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HomeWallet_API.Models.ProductCategory", b =>
                {
                    b.HasOne("HomeWallet_API.Models.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HomeWallet_API.Models.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HomeWallet_API.Models.Receipt", b =>
                {
                    b.HasOne("HomeWallet_API.Models.Shop", "Shop")
                        .WithMany("Receipts")
                        .HasForeignKey("ShopID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HomeWallet_API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HomeWallet_API.Models.ReceiptProduct", b =>
                {
                    b.HasOne("HomeWallet_API.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HomeWallet_API.Models.Receipt", "Receipt")
                        .WithMany("ReceiptProducts")
                        .HasForeignKey("ReceiptID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HomeWallet_API.Models.Shop", b =>
                {
                    b.HasOne("HomeWallet_API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
