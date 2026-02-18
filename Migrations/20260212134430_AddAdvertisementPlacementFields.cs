using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvertisementPlacementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FadeDurationSeconds",
                table: "Advertisements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDismissible",
                table: "Advertisements",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Placement",
                table: "Advertisements",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Advertisements",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_Placement",
                table: "Advertisements",
                column: "Placement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Advertisements_Placement",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "FadeDurationSeconds",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "IsDismissible",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "Placement",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Advertisements");

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
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 10, 18, 29, 29, 516, DateTimeKind.Utc).AddTicks(5320), new DateTime(2026, 2, 10, 18, 29, 29, 516, DateTimeKind.Utc).AddTicks(5320) });
        }
    }
}
