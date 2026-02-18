using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RelatedId = table.Column<string>(type: "TEXT", nullable: true),
                    JobId = table.Column<string>(type: "TEXT", nullable: true),
                    JobTitle = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedDate",
                table: "Notifications",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

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
        }
    }
}
