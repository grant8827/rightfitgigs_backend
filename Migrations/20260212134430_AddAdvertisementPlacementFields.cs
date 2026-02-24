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
                defaultValue: 0);

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
                values: new object[] { "2026-02-12T13:44:29.6249010Z", "2026-02-12T13:44:29.6249010Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-12T13:44:29Z", "2026-02-12T13:44:29Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-12T13:44:29Z", "2026-02-12T13:44:29Z" });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: "2026-02-12T13:44:29Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: "2026-02-12T13:44:29Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: "2026-02-12T13:44:29Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: "2026-02-12T13:44:29Z");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-12T13:44:29Z", "2026-02-12T13:44:29Z" });

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
                values: new object[] { "2026-02-10T18:29:29.5112330Z", "2026-02-10T18:29:29.5112330Z" });

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
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-10T18:29:29Z", "2026-02-10T18:29:29Z" });
        }
    }
}
