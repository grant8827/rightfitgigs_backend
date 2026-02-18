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
                values: new object[] { new DateTime(2026, 2, 10, 18, 29, 29, 511, DateTimeKind.Utc).AddTicks(2330), new DateTime(2026, 2, 10, 18, 29, 29, 511, DateTimeKind.Utc).AddTicks(2330) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 10, 18, 29, 29, 512, DateTimeKind.Utc).AddTicks(210), new DateTime(2026, 2, 10, 18, 29, 29, 512, DateTimeKind.Utc).AddTicks(210) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 10, 18, 29, 29, 512, DateTimeKind.Utc).AddTicks(230), new DateTime(2026, 2, 10, 18, 29, 29, 512, DateTimeKind.Utc).AddTicks(240) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 10, 18, 29, 29, 514, DateTimeKind.Utc).AddTicks(6390));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 10, 18, 29, 29, 515, DateTimeKind.Utc).AddTicks(8380));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 10, 18, 29, 29, 515, DateTimeKind.Utc).AddTicks(8420));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 10, 18, 29, 29, 515, DateTimeKind.Utc).AddTicks(8440));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "IsAdmin", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 10, 18, 29, 29, 516, DateTimeKind.Utc).AddTicks(5320), true, new DateTime(2026, 2, 10, 18, 29, 29, 516, DateTimeKind.Utc).AddTicks(5320) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
