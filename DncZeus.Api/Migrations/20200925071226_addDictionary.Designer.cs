﻿// <auto-generated />
using System;
using DncZeus.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DncZeus.Api.Migrations
{
    [DbContext(typeof(DncZeusDbContext))]
    [Migration("20200925071226_addDictionary")]
    partial class addDictionary
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DncZeus.Api.Entities.DncIcon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Color")
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Custom")
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Size")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DncIcon");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncMenu", b =>
                {
                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Alias")
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("BeforeCloseFun")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Component")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(800)");

                    b.Property<int?>("HideInMenu")
                        .HasColumnType("int");

                    b.Property<string>("Icon")
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("IsDefaultRouter")
                        .HasColumnType("int");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("NotCache")
                        .HasColumnType("int");

                    b.Property<Guid?>("ParentGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ParentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Guid");

                    b.ToTable("DncMenu");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncPermission", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ActionCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(80)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Icon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<Guid>("MenuGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Code");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("MenuGuid");

                    b.ToTable("DncPermission");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncPermissionWithMenu", b =>
                {
                    b.Property<int>("IsDefaultRouter")
                        .HasColumnType("int");

                    b.Property<string>("MenuAlias")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MenuGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MenuName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PermissionActionCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PermissionCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PermissionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PermissionType")
                        .HasColumnType("int");

                    b.ToTable("DncPermissionWithMenu");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncRole", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsBuiltin")
                        .HasColumnType("bit");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<bool>("IsSuperAdministrator")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Code");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("DncRole");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncRolePermissionMapping", b =>
                {
                    b.Property<string>("RoleCode")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PermissionCode")
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("RoleCode", "PermissionCode");

                    b.HasIndex("PermissionCode");

                    b.ToTable("DncRolePermissionMapping");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncUser", b =>
                {
                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(800)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<int>("IsLocked")
                        .HasColumnType("int");

                    b.Property<string>("LoginName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Guid");

                    b.ToTable("DncUser");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncUserRoleMapping", b =>
                {
                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RoleCode")
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("UserGuid", "RoleCode");

                    b.HasIndex("RoleCode");

                    b.ToTable("DncUserRoleMapping");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.FinanceAccount", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Account")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DepartmentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Holder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Owner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Code");

                    b.ToTable("FinanceAccount");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.QueryModels.DncPermission.DncPermissionWithAssignProperty", b =>
                {
                    b.Property<string>("ActionCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IsAssigned")
                        .HasColumnType("int");

                    b.Property<Guid?>("MenuGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleCode")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("DncPermissionWithAssignProperty");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.ResumeInfo", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Alipay")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AnimalSign")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Awards")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("County")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DepartmentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Education")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EducationBackgrounds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Height")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HomeInfo")
                        .HasColumnType("int");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Interests")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<int>("JobStatus")
                        .HasColumnType("int");

                    b.Property<string>("Language")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LevelID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("License")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mobile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("NickName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PositionCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Province")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QQ")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RealName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SelfEvaluations")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.Property<string>("Skills")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TEL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telegram")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TypeID")
                        .HasColumnType("int");

                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Wechat")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Weight")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Works")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Years")
                        .HasColumnType("int");

                    b.HasKey("Code");

                    b.ToTable("ResumeInfo");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.SystemDicType", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Code");

                    b.ToTable("SystemDicType");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.SystemDictionary", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Color")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Fixed")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Code");

                    b.ToTable("SystemDictionary");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.UserDepartment", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Company")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("County")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fax")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Friday")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<int>("LevelID")
                        .HasColumnType("int");

                    b.Property<string>("Manager")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Monday")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Province")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Saturday")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortID")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Sunday")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Thursday")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tuesday")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TypeID")
                        .HasColumnType("int");

                    b.Property<string>("Wednesday")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Zone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Code");

                    b.ToTable("UserDepartment");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.UserPosition", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<int>("LevelID")
                        .HasColumnType("int");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortID")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Code");

                    b.ToTable("UserPosition");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.WageInfo", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal?>("AccumulationFund")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Additions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("BaseWage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Bonus")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Commission")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("CreatedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Deductions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DepartmentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("IncomeTax")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<Guid?>("ModifiedByUserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModifiedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("OTDays")
                        .HasColumnType("int");

                    b.Property<decimal?>("OTWage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("PerformanceWage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PositionCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RealName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("ReissueWage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("SocialSecurity")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal?>("Subsidy")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("TotalWage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("WorkDays")
                        .HasColumnType("int");

                    b.HasKey("Code");

                    b.ToTable("WageInfo");
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncPermission", b =>
                {
                    b.HasOne("DncZeus.Api.Entities.DncMenu", "Menu")
                        .WithMany("Permissions")
                        .HasForeignKey("MenuGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncRolePermissionMapping", b =>
                {
                    b.HasOne("DncZeus.Api.Entities.DncPermission", "DncPermission")
                        .WithMany("Roles")
                        .HasForeignKey("PermissionCode")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DncZeus.Api.Entities.DncRole", "DncRole")
                        .WithMany("Permissions")
                        .HasForeignKey("RoleCode")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DncZeus.Api.Entities.DncUserRoleMapping", b =>
                {
                    b.HasOne("DncZeus.Api.Entities.DncRole", "DncRole")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleCode")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DncZeus.Api.Entities.DncUser", "DncUser")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserGuid")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
