﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using yawaflua.ru.Models;

#nullable disable

namespace yawaflua.ru.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231223164820_Migrate321220231944")]
    partial class Migrate321220231944
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("yawaflua.ru.Models.Tables.Blogs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Annotation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("authorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("authorNickname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("dateTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Blogs", "public");
                });

            modelBuilder.Entity("yawaflua.ru.Models.Tables.Comments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("creatorMail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("dateTime")
                        .HasColumnType("bigint");

                    b.Property<int>("postId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Comments", "public");
                });

            modelBuilder.Entity("yawaflua.ru.Models.Tables.Redirects", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("redirectTo")
                        .HasColumnType("text");

                    b.Property<string>("uri")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Redirects");
                });
#pragma warning restore 612, 618
        }
    }
}
