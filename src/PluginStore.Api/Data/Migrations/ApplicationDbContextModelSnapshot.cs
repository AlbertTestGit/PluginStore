﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PluginStore.Api.Data;

#nullable disable

namespace PluginStore.Api.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PluginStore.Api.Models.Plugin", b =>
                {
                    b.Property<int>("PluginId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PluginId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DeveloperKey")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PetrelVersion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PluginId");

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("PluginStore.Api.Models.PluginVersion", b =>
                {
                    b.Property<int>("PluginVersionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PluginVersionId"));

                    b.Property<Guid>("AuthorUserId")
                        .HasColumnType("uuid");

                    b.Property<bool>("Beta")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("Deprecated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GitLink")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HelpFileEn")
                        .HasColumnType("text");

                    b.Property<string>("HelpFileKz")
                        .HasColumnType("text");

                    b.Property<string>("HelpFileRu")
                        .HasColumnType("text");

                    b.Property<int>("PluginId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("PublicationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PluginVersionId");

                    b.HasIndex("AuthorUserId");

                    b.HasIndex("PluginId");

                    b.ToTable("PluginVersions");
                });

            modelBuilder.Entity("PluginStore.Api.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LicenseNumber")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PluginStore.Api.Models.PluginVersion", b =>
                {
                    b.HasOne("PluginStore.Api.Models.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginStore.Api.Models.Plugin", "Plugin")
                        .WithMany("PluginVersions")
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Plugin");
                });

            modelBuilder.Entity("PluginStore.Api.Models.Plugin", b =>
                {
                    b.Navigation("PluginVersions");
                });
#pragma warning restore 612, 618
        }
    }
}
