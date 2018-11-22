﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using PW.DataAccess;
using PW.DataModel.Enums;
using System;

namespace PW.DataAccess.Migrations
{
    [DbContext(typeof(PWContext))]
    [Migration("20181119182121_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PW.DataModel.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Balance");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("PW.DataModel.Entities.AccountTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<decimal>("PayeeBalance");

                    b.Property<int>("PayeeId");

                    b.Property<decimal>("RecipientBalance");

                    b.Property<int?>("RecipientId");

                    b.Property<DateTime>("TimeOfTransaction");

                    b.HasKey("Id");

                    b.HasIndex("PayeeId");

                    b.HasIndex("RecipientId");

                    b.ToTable("AccountTransactions");
                });

            modelBuilder.Entity("PW.DataModel.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired();

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PW.DataModel.Entities.Account", b =>
                {
                    b.HasOne("PW.DataModel.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PW.DataModel.Entities.AccountTransaction", b =>
                {
                    b.HasOne("PW.DataModel.Entities.Account", "Payee")
                        .WithMany()
                        .HasForeignKey("PayeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PW.DataModel.Entities.Account", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId");
                });
#pragma warning restore 612, 618
        }
    }
}
