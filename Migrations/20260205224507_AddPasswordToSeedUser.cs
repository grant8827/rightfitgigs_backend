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
                values: new object[] { new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(4500), new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(4500) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(8520), new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(8520) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(8540), new DateTime(2026, 2, 5, 22, 45, 6, 737, DateTimeKind.Utc).AddTicks(8540) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 45, 6, 738, DateTimeKind.Utc).AddTicks(8520));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(2800));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(2810));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(4710), "$2a$11$eXUE2pctmN2lvrGRJfJiTOLfL02cUcjuY2tnsjG./iopZ6GafngO6", new DateTime(2026, 2, 5, 22, 45, 6, 739, DateTimeKind.Utc).AddTicks(4710) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-1-tech",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(2240), new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(2240) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-2-mobile",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(6470), new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(6470) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: "company-3-design",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(6480), new DateTime(2026, 2, 5, 22, 34, 40, 132, DateTimeKind.Utc).AddTicks(6490) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-1-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 34, 40, 133, DateTimeKind.Utc).AddTicks(7280));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-2-ios",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(1920));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-3-designer",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(1930));

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: "job-4-senior-flutter",
                column: "UpdatedDate",
                value: new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(1940));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-1-john",
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(3730), "", new DateTime(2026, 2, 5, 22, 34, 40, 134, DateTimeKind.Utc).AddTicks(3730) });
        }
    }
}
