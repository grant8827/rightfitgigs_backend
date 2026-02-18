using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Industry = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Size = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Website = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Company = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Salary = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Industry = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ExperienceLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsRemote = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUrgentlyHiring = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSeasonal = table.Column<bool>(type: "INTEGER", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompanyId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Skills = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserType = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ResumeUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DesiredJobTitle = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DesiredLocation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DesiredSalaryRange = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DesiredJobType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DesiredExperienceLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    OpenToRemote = table.Column<bool>(type: "INTEGER", nullable: false),
                    PreferredIndustries = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompanyId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SenderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SenderName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SenderType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ReceiverId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ReceiverName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ReceiverType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    JobId = table.Column<string>(type: "TEXT", nullable: true),
                    ConversationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "CreatedDate", "Description", "Email", "Industry", "IsActive", "Location", "Name", "Size", "UpdatedDate", "Website" },
                values: new object[,]
                {
                    { "company-1-tech", new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(2240), "Leading technology company specializing in mobile and web solutions", "hr@techcorp.com", "Technology", true, "San Francisco", "Tech Corp", "100-500", new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(2240), "https://techcorp.com" },
                    { "company-2-mobile", new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(6470), "Mobile-first development company", "careers@mobilesolutions.com", "Technology", true, "Remote", "Mobile Solutions", "50-100", new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(6470), "https://mobilesolutions.com" },
                    { "company-3-design", new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(6480), "Creative design agency for digital products", "jobs@designstudio.com", "Design", true, "New York", "Design Studio", "10-50", new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(6490), "https://designstudio.com" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Bio", "CompanyId", "CreatedDate", "DesiredExperienceLevel", "DesiredJobTitle", "DesiredJobType", "DesiredLocation", "DesiredSalaryRange", "Email", "FirstName", "IsActive", "LastName", "Location", "OpenToRemote", "PasswordHash", "Phone", "PreferredIndustries", "ResumeUrl", "Skills", "Title", "UpdatedDate", "UserType" },
                values: new object[] { "user-1-john", "Experienced Flutter developer with passion for mobile development", null, new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(3730), null, null, null, null, null, "john.smith@example.com", "John", true, "Smith", "San Francisco", true, "", "555-0101", null, null, "Flutter, Dart, iOS, Android, Firebase", "Senior Flutter Developer", new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(3730), "Worker" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Company", "CompanyId", "Description", "ExperienceLevel", "Industry", "IsActive", "IsRemote", "IsSeasonal", "IsUrgentlyHiring", "Location", "PostedDate", "Salary", "Title", "Type", "UpdatedDate" },
                values: new object[,]
                {
                    { "job-1-flutter", "Tech Corp", "company-1-tech", "Looking for an experienced Flutter developer to join our team.", null, null, true, false, false, false, "Remote", new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), "$80k - $120k", "Flutter Developer", "Full-time", new DateTime(2026, 2, 5, 22, 34, 40, 133, DateTimeKind.Utc).AddTicks(7280) },
                    { "job-2-ios", "Mobile Solutions", "company-2-mobile", "Native iOS development position with competitive benefits.", null, null, true, false, false, false, "San Francisco", new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), "$90k - $130k", "iOS Developer", "Full-time", new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(1920) },
                    { "job-3-designer", "Design Studio", "company-3-design", "Creative designer needed for mobile and web applications.", null, null, true, false, false, false, "New York", new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), "$70k - $100k", "UI/UX Designer", "Contract", new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(1930) },
                    { "job-4-senior-flutter", "Tech Corp", "company-1-tech", "We are looking for an experienced Flutter developer with 5+ years of experience.", null, null, true, false, false, false, "Remote", new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc), "$100k - $140k", "Senior Flutter Developer", "Full-time", new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(1940) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Industry",
                table: "Companies",
                column: "Industry");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Location",
                table: "Companies",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CompanyId",
                table: "Jobs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Location",
                table: "Jobs",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PostedDate",
                table: "Jobs",
                column: "PostedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Title",
                table: "Jobs",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Type",
                table: "Jobs",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsRead",
                table: "Messages",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_JobId",
                table: "Messages",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentDate",
                table: "Messages",
                column: "SentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Location",
                table: "Users",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserType",
                table: "Users",
                column: "UserType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
