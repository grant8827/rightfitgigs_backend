using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordToSeedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-05T22:45:06.7374500Z", "2026-02-05T22:45:06.7374500Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-05T22:45:06.7378520Z", "2026-02-05T22:45:06.7378520Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-05T22:45:06.7378540Z", "2026-02-05T22:45:06.7378540Z" });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: "2026-02-05T22:45:06.7388520Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: "2026-02-05T22:45:06.7392790Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: "2026-02-05T22:45:06.7392800Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: "2026-02-05T22:45:06.7392810Z");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { "2026-02-05T22:45:06.7394710Z", "$2a$11$eXUE2pctmN2lvrGRJfJiTOLfL02cUcjuY2tnsjG./iopZ6GafngO6", "2026-02-05T22:45:06.7394710Z" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-05T22:34:40.1324200Z", "2026-02-05T22:34:40.1324200Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-05T22:34:40.1328470Z", "2026-02-05T22:34:40.1328470Z" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { "2026-02-05T22:34:40.1328490Z", "2026-02-05T22:34:40.1328490Z" });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: "2026-02-05T22:34:40.1338510Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: "2026-02-05T22:34:40.1342780Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: "2026-02-05T22:34:40.1342790Z");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: "2026-02-05T22:34:40.1342800Z");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { "2026-02-05T22:34:40.1344710Z", "", "2026-02-05T22:34:40.1344710Z" });
        }
    }
}
