using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetExpiry",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "Jobs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployerId",
                table: "Jobs",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 3, 20, 12, 52, 8, 927, DateTimeKind.Utc).AddTicks(750), new DateTime(2026, 3, 20, 12, 52, 8, 927, DateTimeKind.Utc).AddTicks(750) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 3, 20, 12, 52, 8, 927, DateTimeKind.Utc).AddTicks(7040), new DateTime(2026, 3, 20, 12, 52, 8, 927, DateTimeKind.Utc).AddTicks(7040) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 3, 20, 12, 52, 8, 927, DateTimeKind.Utc).AddTicks(7070), new DateTime(2026, 3, 20, 12, 52, 8, 927, DateTimeKind.Utc).AddTicks(7070) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                columns: new[] { "EducationLevel", "EmployerId", "UpdatedDate" },
                values: new object[] { null, null, new DateTime(2026, 3, 20, 12, 52, 8, 929, DateTimeKind.Utc).AddTicks(4010) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                columns: new[] { "EducationLevel", "EmployerId", "UpdatedDate" },
                values: new object[] { null, null, new DateTime(2026, 3, 20, 12, 52, 8, 930, DateTimeKind.Utc).AddTicks(980) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                columns: new[] { "EducationLevel", "EmployerId", "UpdatedDate" },
                values: new object[] { null, null, new DateTime(2026, 3, 20, 12, 52, 8, 930, DateTimeKind.Utc).AddTicks(1000) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                columns: new[] { "EducationLevel", "EmployerId", "UpdatedDate" },
                values: new object[] { null, null, new DateTime(2026, 3, 20, 12, 52, 8, 930, DateTimeKind.Utc).AddTicks(1150) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "EducationLevel", "PasswordResetExpiry", "PasswordResetToken", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 3, 20, 12, 52, 8, 930, DateTimeKind.Utc).AddTicks(3740), null, null, null, new DateTime(2026, 3, 20, 12, 52, 8, 930, DateTimeKind.Utc).AddTicks(3740) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "Jobs");

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
    }
}
