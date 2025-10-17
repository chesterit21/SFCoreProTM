using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SFCoreProTM.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "estimate_points",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstimateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_points", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issue_assignees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssigneeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_assignees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issue_comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    comment_stripped = table.Column<string>(type: "text", nullable: true),
                    CommentJson = table.Column<string>(type: "jsonb", nullable: false),
                    Comment = table.Column<string>(type: "jsonb", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Access = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    external_source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    external_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    attachments = table.Column<string>(type: "jsonb", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issue_description_versions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "jsonb", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_description_versions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issue_labels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabelId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issue_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LogoProperties = table.Column<string>(type: "jsonb", nullable: false),
                    IsEpic = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<double>(type: "double precision", nullable: false),
                    ExternalReference = table.Column<string>(type: "jsonb", nullable: true),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issue_user_properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Preferences = table.Column<string>(type: "jsonb", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_user_properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issue_versions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    StateId = table.Column<Guid>(type: "uuid", nullable: true),
                    EstimatePointId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Priority = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SequenceId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<double>(type: "double precision", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    external_source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    external_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IssueTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CycleId = table.Column<Guid>(type: "uuid", nullable: true),
                    properties = table.Column<string>(type: "jsonb", nullable: false),
                    meta = table.Column<string>(type: "jsonb", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    assignees = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    labels = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    modules = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_versions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    StateId = table.Column<Guid>(type: "uuid", nullable: true),
                    PointEstimate = table.Column<int>(type: "integer", nullable: true),
                    EstimatePointId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "jsonb", nullable: false),
                    Priority = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Schedule = table.Column<string>(type: "jsonb", nullable: false),
                    SequenceId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<double>(type: "double precision", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    external_source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    external_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IssueTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "labels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExternalReference = table.Column<string>(type: "jsonb", nullable: true),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project_identifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_identifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project_issue_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_issue_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project_member_invites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    accepted = table.Column<bool>(type: "boolean", nullable: false),
                    token = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_member_invites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "jsonb", nullable: false),
                    DescriptionRichText = table.Column<string>(type: "jsonb", nullable: false),
                    DescriptionHtml = table.Column<string>(type: "jsonb", nullable: false),
                    Visibility = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Identifier = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    DefaultAssigneeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectLeadId = table.Column<Guid>(type: "uuid", nullable: true),
                    Emoji = table.Column<string>(type: "text", nullable: true),
                    IconProperties = table.Column<string>(type: "jsonb", nullable: false),
                    ModuleViewEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CycleViewEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IssueViewsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PageViewEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IntakeViewEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TimeTrackingEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IssueTypeEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    GuestViewAllFeatures = table.Column<bool>(type: "boolean", nullable: false),
                    cover_image = table.Column<string>(type: "text", nullable: true),
                    cover_image_asset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    EstimateId = table.Column<Guid>(type: "uuid", nullable: true),
                    ArchiveInMonths = table.Column<int>(type: "integer", nullable: false),
                    CloseInMonths = table.Column<int>(type: "integer", nullable: false),
                    LogoProperties = table.Column<string>(type: "jsonb", nullable: false),
                    DefaultStateId = table.Column<Guid>(type: "uuid", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Timezone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExternalReference = table.Column<string>(type: "jsonb", nullable: true),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsTriage = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    ExternalReference = table.Column<string>(type: "jsonb", nullable: true),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_states", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Theme = table.Column<string>(type: "jsonb", nullable: false),
                    IsAppRailDocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsTourCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    OnboardingStep = table.Column<string>(type: "jsonb", nullable: false),
                    UseCase = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    IsOnboarded = table.Column<bool>(type: "boolean", nullable: false),
                    LastWorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    BillingAddressCountry = table.Column<string>(type: "text", nullable: true),
                    BillingAddress = table.Column<string>(type: "jsonb", nullable: false),
                    HasBillingAddress = table.Column<bool>(type: "boolean", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    IsSmoothCursorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsMobileOnboarded = table.Column<bool>(type: "boolean", nullable: false),
                    MobileOnboardingStep = table.Column<string>(type: "jsonb", nullable: false),
                    MobileTimezoneAutoSet = table.Column<bool>(type: "boolean", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: true),
                    StartOfTheWeek = table.Column<int>(type: "integer", nullable: false),
                    Goals = table.Column<string>(type: "jsonb", nullable: false),
                    BackgroundColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    HasMarketingEmailConsent = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "workspace_member_invites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    accepted = table.Column<bool>(type: "boolean", nullable: false),
                    token = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_member_invites", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "project_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Preferences = table.Column<string>(type: "jsonb", nullable: false),
                    SortOrder = table.Column<double>(type: "double precision", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_project_members_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Team_workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workspace_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CompanyRole = table.Column<string>(type: "text", nullable: true),
                    Preferences = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AuditTrail = table.Column<string>(type: "jsonb", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workspace_members_workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_issue_description_versions_timeline",
                table: "issue_description_versions",
                columns: new[] { "ProjectId", "IssueId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "ix_issue_versions_project_issue_sequence",
                table: "issue_versions",
                columns: new[] { "ProjectId", "IssueId", "SequenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_project_members_ProjectId",
                table: "project_members",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_WorkspaceId",
                table: "Team",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_members_WorkspaceId",
                table: "workspace_members",
                column: "WorkspaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "estimate_points");

            migrationBuilder.DropTable(
                name: "issue_assignees");

            migrationBuilder.DropTable(
                name: "issue_comments");

            migrationBuilder.DropTable(
                name: "issue_description_versions");

            migrationBuilder.DropTable(
                name: "issue_labels");

            migrationBuilder.DropTable(
                name: "issue_types");

            migrationBuilder.DropTable(
                name: "issue_user_properties");

            migrationBuilder.DropTable(
                name: "issue_versions");

            migrationBuilder.DropTable(
                name: "issues");

            migrationBuilder.DropTable(
                name: "labels");

            migrationBuilder.DropTable(
                name: "project_identifiers");

            migrationBuilder.DropTable(
                name: "project_issue_types");

            migrationBuilder.DropTable(
                name: "project_member_invites");

            migrationBuilder.DropTable(
                name: "project_members");

            migrationBuilder.DropTable(
                name: "states");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "workspace_member_invites");

            migrationBuilder.DropTable(
                name: "workspace_members");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "workspaces");
        }
    }
}
