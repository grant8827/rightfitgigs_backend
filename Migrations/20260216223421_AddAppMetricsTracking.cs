using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAppMetricsTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppMetrics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MetricType = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMetrics", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 16, 22, 34, 20, 885, DateTimeKind.Utc).AddTicks(7250), new DateTime(2026, 2, 16, 22, 34, 20, 885, DateTimeKind.Utc).AddTicks(7250) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 16, 22, 34, 20, 886, DateTimeKind.Utc).AddTicks(1110), new DateTime(2026, 2, 16, 22, 34, 20, 886, DateTimeKind.Utc).AddTicks(1110) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 16, 22, 34, 20, 886, DateTimeKind.Utc).AddTicks(1130), new DateTime(2026, 2, 16, 22, 34, 20, 886, DateTimeKind.Utc).AddTicks(1130) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 16, 22, 34, 20, 887, DateTimeKind.Utc).AddTicks(320));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 16, 22, 34, 20, 887, DateTimeKind.Utc).AddTicks(5390));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 16, 22, 34, 20, 887, DateTimeKind.Utc).AddTicks(5400));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 16, 22, 34, 20, 887, DateTimeKind.Utc).AddTicks(5410));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 16, 22, 34, 20, 887, DateTimeKind.Utc).AddTicks(6980), new DateTime(2026, 2, 16, 22, 34, 20, 887, DateTimeKind.Utc).AddTicks(6980) });

            migrationBuilder.CreateIndex(
                name: "IX_AppMetrics_CreatedDate",
                table: "AppMetrics",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AppMetrics_MetricType",
                table: "AppMetrics",
                column: "MetricType");

            migrationBuilder.CreateIndex(
                name: "IX_AppMetrics_Platform",
                table: "AppMetrics",
                column: "Platform");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppMetrics");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 12, 13, 44, 29, 624, DateTimeKind.Utc).AddTicks(9010), new DateTime(2026, 2, 12, 13, 44, 29, 624, DateTimeKind.Utc).AddTicks(9010) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 12, 13, 44, 29, 625, DateTimeKind.Utc).AddTicks(2700), new DateTime(2026, 2, 12, 13, 44, 29, 625, DateTimeKind.Utc).AddTicks(2710) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 12, 13, 44, 29, 625, DateTimeKind.Utc).AddTicks(2720), new DateTime(2026, 2, 12, 13, 44, 29, 625, DateTimeKind.Utc).AddTicks(2720) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 12, 13, 44, 29, 626, DateTimeKind.Utc).AddTicks(2620));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 12, 13, 44, 29, 626, DateTimeKind.Utc).AddTicks(6410));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 12, 13, 44, 29, 626, DateTimeKind.Utc).AddTicks(6410));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 12, 13, 44, 29, 626, DateTimeKind.Utc).AddTicks(6420));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 12, 13, 44, 29, 626, DateTimeKind.Utc).AddTicks(7880), new DateTime(2026, 2, 12, 13, 44, 29, 626, DateTimeKind.Utc).AddTicks(7880) });
        }
    }
}
