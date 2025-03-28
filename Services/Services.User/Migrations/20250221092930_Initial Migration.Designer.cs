﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Services.User.Db;

#nullable disable

namespace Services.User.Migrations
{
    [DbContext(typeof(UserDbContext))]
    [Migration("20250221092930_Initial Migration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Infrastructure.Messaging.Outbox.Domain.OutboxMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<string>("MessageType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message_type");

                    b.Property<DateTimeOffset?>("SentAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("sent_at");

                    b.HasKey("Id");

                    b.ToTable("outbox");
                });

            modelBuilder.Entity("Services.User.Domain.OrderRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CustomerAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("customer_address");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("customer_name");

                    b.HasKey("Id");

                    b.ToTable("order_requests");
                });

            modelBuilder.Entity("Services.User.Domain.OrderRequestItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Amount")
                        .HasColumnType("integer")
                        .HasColumnName("amount");

                    b.Property<string>("ArticleName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("article_name");

                    b.Property<decimal>("ArticlePrice")
                        .HasColumnType("numeric")
                        .HasColumnName("article_price");

                    b.Property<Guid?>("OrderRequestId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OrderRequestId");

                    b.ToTable("order_request_items");
                });

            modelBuilder.Entity("Services.User.Domain.OrderRequestItem", b =>
                {
                    b.HasOne("Services.User.Domain.OrderRequest", null)
                        .WithMany("Items")
                        .HasForeignKey("OrderRequestId");
                });

            modelBuilder.Entity("Services.User.Domain.OrderRequest", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
