﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlanIT.API.Data;

#nullable disable

namespace PlanIT.API.Data.Migrations
{
    [DbContext(typeof(PlanITDbContext))]
    [Migration("20240418090724_PlanITMigrations")]
    partial class PlanITMigrations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("PlanIT.API.Models.Entities.Contact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.Dinner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Dinners");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<TimeOnly>("Time")
                        .HasColumnType("time(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.ImportantDate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ImportantDates");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.Invite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Coming")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<bool>("IsReminderSent")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("EventId");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.ShoppingList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ShoppingLists");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.ToDo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Todos");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "perhansen@mail.com",
                            HashedPassword = "$2a$11$SDAc068CAM7zbUHHBGiYl.bhji7vr2ujZSpPuNuqApCMneFD7/qee",
                            Name = "Per",
                            Salt = "$2a$11$SDAc068CAM7zbUHHBGiYl."
                        },
                        new
                        {
                            Id = 2,
                            Email = "olanordmann@mail.com",
                            HashedPassword = "$2a$11$SDAc068CAM7zbUHHBGiYl.bhji7vr2ujZSpPuNuqApCMneFD7/qee",
                            Name = "Ola",
                            Salt = "$2a$11$SDAc068CAM7zbUHHBGiYl."
                        },
                        new
                        {
                            Id = 3,
                            Email = "karinordmann@mail.com",
                            HashedPassword = "$2a$11$SDAc068CAM7zbUHHBGiYl.6swBwBK6lLes1.yoFgz0D2D2vLGv7Se",
                            Name = "Kari",
                            Salt = "$2a$11$SDAc068CAM7zbUHHBGiYl."
                        });
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.Contact", b =>
                {
                    b.HasOne("PlanIT.API.Models.Entities.User", "User")
                        .WithMany("Contacts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.Dinner", b =>
                {
                    b.HasOne("PlanIT.API.Models.Entities.User", "User")
                        .WithMany("Dinners")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.Event", b =>
                {
                    b.HasOne("PlanIT.API.Models.Entities.User", "User")
                        .WithMany("Events")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.ImportantDate", b =>
                {
                    b.HasOne("PlanIT.API.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.Invite", b =>
                {
                    b.HasOne("PlanIT.API.Models.Entities.Event", "Event")
                        .WithMany("Invites")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.ShoppingList", b =>
                {
                    b.HasOne("PlanIT.API.Models.Entities.User", "User")
                        .WithMany("ShoppingList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.ToDo", b =>
                {
                    b.HasOne("PlanIT.API.Models.Entities.User", "User")
                        .WithMany("Todos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.Event", b =>
                {
                    b.Navigation("Invites");
                });

            modelBuilder.Entity("PlanIT.API.Models.Entities.User", b =>
                {
                    b.Navigation("Contacts");

                    b.Navigation("Dinners");

                    b.Navigation("Events");

                    b.Navigation("ShoppingList");

                    b.Navigation("Todos");
                });
#pragma warning restore 612, 618
        }
    }
}