using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    JobId = table.Column<string>(type: "TEXT", nullable: false),
                    WorkerId = table.Column<string>(type: "TEXT", nullable: false),
                    WorkerName = table.Column<string>(type: "TEXT", nullable: false),
                    WorkerEmail = table.Column<string>(type: "TEXT", nullable: false),
                    WorkerPhone = table.Column<string>(type: "TEXT", nullable: false),
                    WorkerSkills = table.Column<string>(type: "TEXT", nullable: false),
                    WorkerTitle = table.Column<string>(type: "TEXT", nullable: false),
                    WorkerLocation = table.Column<string>(type: "TEXT", nullable: false),
                    ResumeUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CoverLetter = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Users_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 23, 12, 13, 314, DateTimeKind.Utc).AddTicks(1930), new DateTime(2026, 2, 5, 23, 12, 13, 314, DateTimeKind.Utc).AddTicks(1930) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 23, 12, 13, 314, DateTimeKind.Utc).AddTicks(6420), new DateTime(2026, 2, 5, 23, 12, 13, 314, DateTimeKind.Utc).AddTicks(6420) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 23, 12, 13, 314, DateTimeKind.Utc).AddTicks(6440), new DateTime(2026, 2, 5, 23, 12, 13, 314, DateTimeKind.Utc).AddTicks(6440) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 23, 12, 13, 315, DateTimeKind.Utc).AddTicks(8330));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 23, 12, 13, 316, DateTimeKind.Utc).AddTicks(3290));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 23, 12, 13, 316, DateTimeKind.Utc).AddTicks(3300));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 23, 12, 13, 316, DateTimeKind.Utc).AddTicks(3310));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 23, 12, 13, 316, DateTimeKind.Utc).AddTicks(6910), new DateTime(2026, 2, 5, 23, 12, 13, 316, DateTimeKind.Utc).AddTicks(6910) });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobId",
                table: "Applications",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_WorkerId",
                table: "Applications",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(4500), new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(4500) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(8520), new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(8520) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(8540), new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(8540) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 45, 6, 738, DateTimeKind.Utc).AddTicks(8520));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(2800));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(2810));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(4710), new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(4710) });
        }
    }
}
