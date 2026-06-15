using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MorgueManager.API.Migrations
{
    /// <inheritdoc />
    public partial class ComprehensiveUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StorageSlotId",
                table: "Corpses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentTemperature = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemperatureLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StorageSlotId = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "StorageSlots",
                columns: new[] { "Id", "CurrentTemperature", "SlotNumber", "Status", "UnitName" },
                values: new object[,]
                {
                    { 1, 4.0, "A-01", 0, "Cold Room A" },
                    { 2, 4.0, "A-02", 0, "Cold Room A" },
                    { 3, 4.0, "A-03", 0, "Cold Room A" },
                    { 4, 4.0, "A-04", 0, "Cold Room A" },
                    { 5, 4.0, "A-05", 0, "Cold Room A" },
                    { 6, 4.0, "A-06", 0, "Cold Room A" },
                    { 7, 4.0, "A-07", 0, "Cold Room A" },
                    { 8, 4.0, "A-08", 0, "Cold Room A" },
                    { 9, 4.0, "A-09", 0, "Cold Room A" },
                    { 10, 4.0, "A-10", 0, "Cold Room A" },
                    { 11, 4.0, "B-01", 0, "Cold Room B" },
                    { 12, 4.0, "B-02", 0, "Cold Room B" },
                    { 13, 4.0, "B-03", 0, "Cold Room B" },
                    { 14, 4.0, "B-04", 0, "Cold Room B" },
                    { 15, 4.0, "B-05", 0, "Cold Room B" },
                    { 16, 4.0, "B-06", 0, "Cold Room B" },
                    { 17, 4.0, "B-07", 0, "Cold Room B" },
                    { 18, 4.0, "B-08", 0, "Cold Room B" },
                    { 19, 4.0, "B-09", 0, "Cold Room B" },
                    { 20, 4.0, "B-10", 0, "Cold Room B" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin User", "admin@hospital.vn", "$2a$11$V6VVNz2xMBegNqXAB8HJNuBpYZFm9DClOl.fpQsvdB/up2G6TGK.W", "Admin" },
                    { 2, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manager User", "manager@hospital.vn", "$2a$11$V6VVNz2xMBegNqXAB8HJNuBpYZFm9DClOl.fpQsvdB/up2G6TGK.W", "Manager" },
                    { 3, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Staff User", "staff@hospital.vn", "$2a$11$V6VVNz2xMBegNqXAB8HJNuBpYZFm9DClOl.fpQsvdB/up2G6TGK.W", "Staff" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "StorageSlots");

            migrationBuilder.DropTable(
                name: "TemperatureLogs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "StorageSlotId",
                table: "Corpses");
        }
    }
}
