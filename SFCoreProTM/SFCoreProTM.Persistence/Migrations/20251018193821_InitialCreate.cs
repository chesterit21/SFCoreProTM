using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SFCoreProTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "erd_definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    EntityName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AttributeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AttributeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsPrimaryKey = table.Column<bool>(type: "boolean", nullable: false),
                    IsAcceptNull = table.Column<bool>(type: "boolean", nullable: false),
                    MaxChar = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ErdStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_erd_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "flow_tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    FlowStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flow_tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ProjectPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sprint_plannings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    SprintStatus = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sprint_plannings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsErd = table.Column<bool>(type: "boolean", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Theme = table.Column<string>(type: "jsonb", nullable: true),
                    IsAppRailDocked = table.Column<bool>(type: "boolean", nullable: true),
                    IsTourCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    OnboardingStep = table.Column<string>(type: "jsonb", nullable: true),
                    UseCase = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    IsOnboarded = table.Column<bool>(type: "boolean", nullable: true),
                    LastWorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    BillingAddressCountry = table.Column<string>(type: "text", nullable: true),
                    BillingAddress = table.Column<string>(type: "jsonb", nullable: true),
                    HasBillingAddress = table.Column<bool>(type: "boolean", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    IsSmoothCursorEnabled = table.Column<bool>(type: "boolean", nullable: true),
                    IsMobileOnboarded = table.Column<bool>(type: "boolean", nullable: true),
                    MobileOnboardingStep = table.Column<string>(type: "jsonb", nullable: true),
                    MobileTimezoneAutoSet = table.Column<bool>(type: "boolean", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: true),
                    StartOfTheWeek = table.Column<int>(type: "integer", nullable: true),
                    Goals = table.Column<string>(type: "jsonb", nullable: true),
                    BackgroundColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    HasMarketingEmailConsent = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    mobile_number = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    display_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    last_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    avatar = table.Column<string>(type: "text", nullable: true),
                    avatar_asset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cover_image = table.Column<string>(type: "text", nullable: true),
                    cover_image_asset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_joined = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_location = table.Column<string>(type: "text", nullable: true),
                    created_location = table.Column<string>(type: "text", nullable: true),
                    is_superuser = table.Column<bool>(type: "boolean", nullable: false),
                    is_managed = table.Column<bool>(type: "boolean", nullable: false),
                    is_password_expired = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_staff = table.Column<bool>(type: "boolean", nullable: false),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    is_password_autoset = table.Column<bool>(type: "boolean", nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    token = table.Column<string>(type: "text", nullable: false),
                    last_active = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_logout_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_ip = table.Column<string>(type: "text", nullable: true),
                    last_logout_ip = table.Column<string>(type: "text", nullable: true),
                    last_login_medium = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    last_login_uagent = table.Column<string>(type: "text", nullable: true),
                    token_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_bot = table.Column<bool>(type: "boolean", nullable: false),
                    bot_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    user_timezone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_email_valid = table.Column<bool>(type: "boolean", nullable: false),
                    masked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    logo = table.Column<string>(type: "text", nullable: true),
                    logo_asset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    organization_size = table.Column<string>(type: "text", nullable: true),
                    Timezone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    background_color = table.Column<string>(type: "text", nullable: true),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspaces", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "erd_definitions");

            migrationBuilder.DropTable(
                name: "flow_tasks");

            migrationBuilder.DropTable(
                name: "modules");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "sprint_plannings");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "workspaces");
        }
    }
}
