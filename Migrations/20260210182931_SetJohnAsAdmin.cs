using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class SetJohnAsAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:29:29Z", "2026-02-10T18:29:29Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:29:29Z", "2026-02-10T18:29:29Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:29:29Z", "2026-02-10T18:29:29Z" });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: "2026-02-10T18:29:29Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: "2026-02-10T18:29:29Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: "2026-02-10T18:29:29Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: "2026-02-10T18:29:29Z");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "IsAdmin", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:29:29Z", 1, "2026-02-10T18:29:29Z" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:00:33.2364510Z", "2026-02-10T18:00:33.2364510Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:00:33.2372810Z", "2026-02-10T18:00:33.2372810Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:00:33.2372840Z", "2026-02-10T18:00:33.2372840Z" });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: "2026-02-10T18:00:33.2394750Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: "2026-02-10T18:00:33.2407160Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: "2026-02-10T18:00:33.2407200Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: "2026-02-10T18:00:33.2407220Z");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "IsAdmin", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:00:33.2413730Z", 0, "2026-02-10T18:00:33.2413730Z" });
        }
    }
}
