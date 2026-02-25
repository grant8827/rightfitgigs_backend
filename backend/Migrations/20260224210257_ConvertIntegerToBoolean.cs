using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ConvertIntegerToBoolean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 24, 21, 2, 52, 958, DateTimeKind.Utc).AddTicks(9620), new DateTime(2026, 2, 24, 21, 2, 52, 958, DateTimeKind.Utc).AddTicks(9620) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 24, 21, 2, 52, 961, DateTimeKind.Utc).AddTicks(2510), new DateTime(2026, 2, 24, 21, 2, 52, 961, DateTimeKind.Utc).AddTicks(2510) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 24, 21, 2, 52, 961, DateTimeKind.Utc).AddTicks(2570), new DateTime(2026, 2, 24, 21, 2, 52, 961, DateTimeKind.Utc).AddTicks(2570) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 24, 21, 2, 52, 965, DateTimeKind.Utc).AddTicks(1480));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 24, 21, 2, 52, 966, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 24, 21, 2, 52, 966, DateTimeKind.Utc).AddTicks(2890));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 24, 21, 2, 52, 966, DateTimeKind.Utc).AddTicks(2910));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 24, 21, 2, 52, 967, DateTimeKind.Utc).AddTicks(1320), new DateTime(2026, 2, 24, 21, 2, 52, 967, DateTimeKind.Utc).AddTicks(1320) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 24, 19, 13, 37, 366, DateTimeKind.Utc).AddTicks(5760), new DateTime(2026, 2, 24, 19, 13, 37, 366, DateTimeKind.Utc).AddTicks(5760) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 24, 19, 13, 37, 368, DateTimeKind.Utc).AddTicks(3800), new DateTime(2026, 2, 24, 19, 13, 37, 368, DateTimeKind.Utc).AddTicks(3800) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 24, 19, 13, 37, 368, DateTimeKind.Utc).AddTicks(3870), new DateTime(2026, 2, 24, 19, 13, 37, 368, DateTimeKind.Utc).AddTicks(3880) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 24, 19, 13, 37, 373, DateTimeKind.Utc).AddTicks(7780));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 24, 19, 13, 37, 395, DateTimeKind.Utc).AddTicks(8380));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 24, 19, 13, 37, 395, DateTimeKind.Utc).AddTicks(8420));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 24, 19, 13, 37, 395, DateTimeKind.Utc).AddTicks(8450));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 24, 19, 13, 37, 396, DateTimeKind.Utc).AddTicks(7600), new DateTime(2026, 2, 24, 19, 13, 37, 396, DateTimeKind.Utc).AddTicks(7600) });
        }
    }
}
