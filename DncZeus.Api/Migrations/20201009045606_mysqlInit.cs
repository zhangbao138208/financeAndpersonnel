using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class mysqlInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DncIcon",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Custom = table.Column<string>(type: "nvarchar(60)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DncIcon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DncMenu",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    ParentGuid = table.Column<Guid>(nullable: true),
                    ParentName = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Description = table.Column<string>(type: "nvarchar(800)", nullable: true),
                    Sort = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    IsDefaultRouter = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    Component = table.Column<string>(maxLength: 255, nullable: true),
                    HideInMenu = table.Column<int>(nullable: true),
                    NotCache = table.Column<int>(nullable: true),
                    BeforeCloseFun = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DncMenu", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "DncPermissionWithAssignProperty",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    MenuGuid = table.Column<Guid>(nullable: true),
                    ActionCode = table.Column<string>(nullable: true),
                    RoleCode = table.Column<string>(nullable: true),
                    IsAssigned = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DncPermissionWithMenu",
                columns: table => new
                {
                    PermissionCode = table.Column<string>(nullable: true),
                    PermissionActionCode = table.Column<string>(nullable: true),
                    PermissionName = table.Column<string>(nullable: true),
                    PermissionType = table.Column<int>(nullable: false),
                    MenuName = table.Column<string>(nullable: true),
                    MenuGuid = table.Column<Guid>(nullable: false),
                    MenuAlias = table.Column<string>(nullable: true),
                    IsDefaultRouter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DncRole",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    IsSuperAdministrator = table.Column<bool>(nullable: false),
                    IsBuiltin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DncRole", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "FinanceAccount",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    Holder = table.Column<string>(nullable: true),
                    DepartmentCode = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Account = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceAccount", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "FinanceInfo",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    User = table.Column<string>(nullable: true),
                    DepartmentCode = table.Column<string>(nullable: true),
                    FinanceAccount = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    InfoStatus = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    HandleName = table.Column<string>(nullable: true),
                    HandleDate = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ImagePath = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceInfo", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ResumeInfo",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    UserGuid = table.Column<Guid>(nullable: false),
                    DepartmentCode = table.Column<string>(nullable: true),
                    PositionCode = table.Column<string>(nullable: true),
                    TypeID = table.Column<int>(nullable: false),
                    JobStatus = table.Column<int>(nullable: false),
                    RealName = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Birthday = table.Column<DateTime>(nullable: true),
                    AnimalSign = table.Column<string>(nullable: true),
                    Age = table.Column<int>(nullable: false),
                    Sex = table.Column<int>(nullable: false),
                    Weight = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    Years = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    License = table.Column<string>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    LevelID = table.Column<string>(nullable: true),
                    Education = table.Column<string>(nullable: true),
                    EducationBackgrounds = table.Column<string>(nullable: true),
                    Works = table.Column<string>(nullable: true),
                    Awards = table.Column<string>(nullable: true),
                    SelfEvaluations = table.Column<string>(nullable: true),
                    Interests = table.Column<string>(nullable: true),
                    Skills = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    TEL = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    Wechat = table.Column<string>(nullable: true),
                    Telegram = table.Column<string>(nullable: true),
                    QQ = table.Column<string>(nullable: true),
                    Alipay = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    County = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    HomeInfo = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeInfo", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "SystemDicType",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemDicType", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UserDepartment",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Phone1 = table.Column<string>(nullable: true),
                    Phone2 = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    ParentCode = table.Column<string>(nullable: true),
                    LevelID = table.Column<int>(nullable: false),
                    SortID = table.Column<int>(nullable: false),
                    TypeID = table.Column<int>(nullable: false),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    County = table.Column<string>(nullable: true),
                    Zone = table.Column<string>(nullable: true),
                    Manager = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    Monday = table.Column<string>(nullable: true),
                    Tuesday = table.Column<string>(nullable: true),
                    Wednesday = table.Column<string>(nullable: true),
                    Thursday = table.Column<string>(nullable: true),
                    Friday = table.Column<string>(nullable: true),
                    Saturday = table.Column<string>(nullable: true),
                    Sunday = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDepartment", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UserPosition",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ParentCode = table.Column<string>(nullable: true),
                    LevelID = table.Column<int>(nullable: false),
                    SortID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPosition", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "WageInfo",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    UserGuid = table.Column<Guid>(nullable: false),
                    RealName = table.Column<string>(nullable: true),
                    DepartmentCode = table.Column<string>(nullable: true),
                    PositionCode = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    BaseWage = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    WorkDays = table.Column<int>(nullable: true),
                    OTWage = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    OTDays = table.Column<int>(nullable: true),
                    PerformanceWage = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ReissueWage = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Commission = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Bonus = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Additions = table.Column<string>(nullable: true),
                    Subsidy = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    SocialSecurity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    AccumulationFund = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IncomeTax = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Deductions = table.Column<string>(nullable: true),
                    TotalWage = table.Column<decimal>(type: "decimal(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WageInfo", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowList",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    User = table.Column<Guid>(nullable: false),
                    DepartmentCode = table.Column<string>(nullable: true),
                    TemplateCode = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CurrentStepCode = table.Column<string>(nullable: true),
                    NextStepCode = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    NotifyUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowList", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowReceiver",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkflowCode = table.Column<string>(nullable: true),
                    TemplateCode = table.Column<string>(nullable: true),
                    StepCode = table.Column<string>(nullable: true),
                    User = table.Column<Guid>(nullable: false),
                    IsCheck = table.Column<bool>(nullable: false),
                    CheckDate = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreateUser = table.Column<Guid>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowReceiver", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStep",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    TemplateCode = table.Column<string>(nullable: true),
                    UserList = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    SortID = table.Column<string>(nullable: true),
                    IsCounterSign = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStep", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTemplate",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentCode = table.Column<string>(nullable: true),
                    Visible = table.Column<bool>(nullable: false),
                    IsStepFree = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTemplate", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "DncPermission",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    MenuGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ActionCode = table.Column<string>(type: "nvarchar(80)", nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DncPermission", x => x.Code);
                    table.ForeignKey(
                        name: "FK_DncPermission_DncMenu_MenuGuid",
                        column: x => x.MenuGuid,
                        principalTable: "DncMenu",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemDictionary",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    TypeCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Fixed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemDictionary", x => x.Code);
                    table.ForeignKey(
                        name: "FK_SystemDictionary_SystemDicType_TypeCode",
                        column: x => x.TypeCode,
                        principalTable: "SystemDicType",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DncUser",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    LoginName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    DepartmentCode = table.Column<string>(nullable: true),
                    PositionCode = table.Column<string>(nullable: true),
                    TelegramBotToken = table.Column<string>(nullable: true),
                    TelegramChatId = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    IsLocked = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(type: "nvarchar(800)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DncUser", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_DncUser_UserDepartment_DepartmentCode",
                        column: x => x.DepartmentCode,
                        principalTable: "UserDepartment",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DncUser_UserDepartment_PositionCode",
                        column: x => x.PositionCode,
                        principalTable: "UserDepartment",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DncRolePermissionMapping",
                columns: table => new
                {
                    RoleCode = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DncRolePermissionMapping", x => new { x.RoleCode, x.PermissionCode });
                    table.ForeignKey(
                        name: "FK_DncRolePermissionMapping_DncPermission_PermissionCode",
                        column: x => x.PermissionCode,
                        principalTable: "DncPermission",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DncRolePermissionMapping_DncRole_RoleCode",
                        column: x => x.RoleCode,
                        principalTable: "DncRole",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DncUserRoleMapping",
                columns: table => new
                {
                    UserGuid = table.Column<Guid>(nullable: false),
                    RoleCode = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DncUserRoleMapping", x => new { x.UserGuid, x.RoleCode });
                    table.ForeignKey(
                        name: "FK_DncUserRoleMapping_DncRole_RoleCode",
                        column: x => x.RoleCode,
                        principalTable: "DncRole",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DncUserRoleMapping_DncUser_UserGuid",
                        column: x => x.UserGuid,
                        principalTable: "DncUser",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DncPermission_Code",
                table: "DncPermission",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DncPermission_MenuGuid",
                table: "DncPermission",
                column: "MenuGuid");

            migrationBuilder.CreateIndex(
                name: "IX_DncRole_Code",
                table: "DncRole",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DncRolePermissionMapping_PermissionCode",
                table: "DncRolePermissionMapping",
                column: "PermissionCode");

            migrationBuilder.CreateIndex(
                name: "IX_DncUser_DepartmentCode",
                table: "DncUser",
                column: "DepartmentCode");

            migrationBuilder.CreateIndex(
                name: "IX_DncUser_PositionCode",
                table: "DncUser",
                column: "PositionCode");

            migrationBuilder.CreateIndex(
                name: "IX_DncUserRoleMapping_RoleCode",
                table: "DncUserRoleMapping",
                column: "RoleCode");

            migrationBuilder.CreateIndex(
                name: "IX_SystemDictionary_Code",
                table: "SystemDictionary",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemDictionary_TypeCode",
                table: "SystemDictionary",
                column: "TypeCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DncIcon");

            migrationBuilder.DropTable(
                name: "DncPermissionWithAssignProperty");

            migrationBuilder.DropTable(
                name: "DncPermissionWithMenu");

            migrationBuilder.DropTable(
                name: "DncRolePermissionMapping");

            migrationBuilder.DropTable(
                name: "DncUserRoleMapping");

            migrationBuilder.DropTable(
                name: "FinanceAccount");

            migrationBuilder.DropTable(
                name: "FinanceInfo");

            migrationBuilder.DropTable(
                name: "ResumeInfo");

            migrationBuilder.DropTable(
                name: "SystemDictionary");

            migrationBuilder.DropTable(
                name: "UserPosition");

            migrationBuilder.DropTable(
                name: "WageInfo");

            migrationBuilder.DropTable(
                name: "WorkflowList");

            migrationBuilder.DropTable(
                name: "WorkflowReceiver");

            migrationBuilder.DropTable(
                name: "WorkflowStep");

            migrationBuilder.DropTable(
                name: "WorkflowTemplate");

            migrationBuilder.DropTable(
                name: "DncPermission");

            migrationBuilder.DropTable(
                name: "DncRole");

            migrationBuilder.DropTable(
                name: "DncUser");

            migrationBuilder.DropTable(
                name: "SystemDicType");

            migrationBuilder.DropTable(
                name: "DncMenu");

            migrationBuilder.DropTable(
                name: "UserDepartment");
        }
    }
}
