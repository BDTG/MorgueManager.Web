using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MorgueManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AdvancedUpgrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutopsyReport_ConcludingCause",
                table: "Corpses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutopsyReport_InternalExamDetails",
                table: "Corpses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutopsyReport_PathologistName",
                table: "Corpses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AutopsyReport_Timestamp",
                table: "Corpses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutopsyReport_ToxicologyResult",
                table: "Corpses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Shifts",
                columns: new[] { "Id", "Date", "Notes", "ShiftType", "StaffEmail" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 2, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 3, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 4, new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 5, new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 6, new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 7, new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 8, new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 9, new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 10, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 11, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 12, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 13, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 14, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 15, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 16, new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 17, new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 18, new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 19, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 20, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 21, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 22, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 23, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 24, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 25, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 26, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 27, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 28, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 29, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 30, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 31, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 32, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 33, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 34, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 35, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 36, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 37, new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 38, new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 39, new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 40, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 41, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 42, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 43, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 44, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 45, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 46, new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 47, new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 48, new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 49, new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 50, new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 51, new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 52, new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 53, new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 54, new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 55, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 56, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 57, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 58, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 59, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 60, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 61, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 62, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 63, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 64, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 65, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 66, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 67, new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 68, new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 69, new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 70, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 71, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 72, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 73, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 74, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 75, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 76, new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 77, new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 78, new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 79, new DateTime(2026, 6, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 80, new DateTime(2026, 6, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 81, new DateTime(2026, 6, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 82, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 83, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 84, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 85, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 86, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 87, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" },
                    { 88, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Ca sáng", "Morning", "staff@hospital.vn" },
                    { 89, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Ca chiều", "Afternoon", "manager@hospital.vn" },
                    { 90, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Ca đêm", "Night", "staff@hospital.vn" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropColumn(
                name: "AutopsyReport_ConcludingCause",
                table: "Corpses");

            migrationBuilder.DropColumn(
                name: "AutopsyReport_InternalExamDetails",
                table: "Corpses");

            migrationBuilder.DropColumn(
                name: "AutopsyReport_PathologistName",
                table: "Corpses");

            migrationBuilder.DropColumn(
                name: "AutopsyReport_Timestamp",
                table: "Corpses");

            migrationBuilder.DropColumn(
                name: "AutopsyReport_ToxicologyResult",
                table: "Corpses");
        }
    }
}
