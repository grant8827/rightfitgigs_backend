using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAndAdvertisements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FileUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TargetUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BusinessName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ClickCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 10, 18, 0, 33, 236, DateTimeKind.Utc).AddTicks(4510), new DateTime(2026, 2, 10, 18, 0, 33, 236, DateTimeKind.Utc).AddTicks(4510) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 10, 18, 0, 33, 237, DateTimeKind.Utc).AddTicks(2810), new DateTime(2026, 2, 10, 18, 0, 33, 237, DateTimeKind.Utc).AddTicks(2810) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 10, 18, 0, 33, 237, DateTimeKind.Utc).AddTicks(2840), new DateTime(2026, 2, 10, 18, 0, 33, 237, DateTimeKind.Utc).AddTicks(2840) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 10, 18, 0, 33, 239, DateTimeKind.Utc).AddTicks(4750));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 10, 18, 0, 33, 240, DateTimeKind.Utc).AddTicks(7160));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 10, 18, 0, 33, 240, DateTimeKind.Utc).AddTicks(7200));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 10, 18, 0, 33, 240, DateTimeKind.Utc).AddTicks(7220));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "IsAdmin", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 10, 18, 0, 33, 241, DateTimeKind.Utc).AddTicks(3730), false, new DateTime(2026, 2, 10, 18, 0, 33, 241, DateTimeKind.Utc).AddTicks(3730) });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_DisplayOrder",
                table: "Advertisements",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_IsActive",
                table: "Advertisements",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_Platform",
                table: "Advertisements",
                column: "Platform");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_StartDate",
                table: "Advertisements",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_Type",
                table: "Advertisements",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 7, 1, 16, 35, 432, DateTimeKind.Utc).AddTicks(2760), new DateTime(2026, 2, 7, 1, 16, 35, 432, DateTimeKind.Utc).AddTicks(2760) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 7, 1, 16, 35, 432, DateTimeKind.Utc).AddTicks(7350), new DateTime(2026, 2, 7, 1, 16, 35, 432, DateTimeKind.Utc).AddTicks(7350) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 7, 1, 16, 35, 432, DateTimeKind.Utc).AddTicks(7370), new DateTime(2026, 2, 7, 1, 16, 35, 432, DateTimeKind.Utc).AddTicks(7370) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 7, 1, 16, 35, 434, DateTimeKind.Utc).AddTicks(290));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 7, 1, 16, 35, 434, DateTimeKind.Utc).AddTicks(6700));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 7, 1, 16, 35, 434, DateTimeKind.Utc).AddTicks(6710));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 7, 1, 16, 35, 434, DateTimeKind.Utc).AddTicks(6720));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 7, 1, 16, 35, 434, DateTimeKind.Utc).AddTicks(9660), new DateTime(2026, 2, 7, 1, 16, 35, 434, DateTimeKind.Utc).AddTicks(9660) });
        }
    }
}
