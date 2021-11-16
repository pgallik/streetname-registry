﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StreetNameRegistry.Projections.Extract;

namespace StreetNameRegistry.Projections.Extract.Migrations
{
    [DbContext(typeof(ExtractContext))]
    partial class ExtractContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.ProjectionStates.ProjectionStateItem", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DesiredState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("DesiredStateChangedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Position")
                        .HasColumnType("bigint");

                    b.HasKey("Name")
                        .IsClustered();

                    b.ToTable("ProjectionStates", "StreetNameRegistryExtract");
                });

            modelBuilder.Entity("StreetNameRegistry.Projections.Extract.StreetNameExtract.StreetNameExtractItem", b =>
                {
                    b.Property<Guid?>("StreetNameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Complete")
                        .HasColumnType("bit");

                    b.Property<byte[]>("DbaseRecord")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("HomonymDutch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomonymEnglish")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomonymFrench")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomonymGerman")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomonymUnknown")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameDutch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameEnglish")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFrench")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameGerman")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameUnknown")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StreetNamePersistentLocalId")
                        .HasColumnType("int");

                    b.HasKey("StreetNameId")
                        .IsClustered(false);

                    b.HasIndex("StreetNamePersistentLocalId")
                        .IsClustered();

                    b.ToTable("StreetName", "StreetNameRegistryExtract");
                });

            modelBuilder.Entity("StreetNameRegistry.Projections.Extract.StreetNameExtract.StreetNameExtractItemV2", b =>
                {
                    b.Property<Guid>("MunicipalityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("StreetNamePersistentLocalId")
                        .HasColumnType("int");

                    b.Property<bool>("Complete")
                        .HasColumnType("bit");

                    b.Property<byte[]>("DbaseRecord")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("HomonymDutch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomonymEnglish")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomonymFrench")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomonymGerman")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameDutch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameEnglish")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFrench")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameGerman")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MunicipalityId", "StreetNamePersistentLocalId")
                        .IsClustered(false);

                    b.HasIndex("StreetNamePersistentLocalId")
                        .IsClustered();

                    b.ToTable("StreetNameV2", "StreetNameRegistryExtract");
                });
#pragma warning restore 612, 618
        }
    }
}
