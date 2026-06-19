using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MorgueManager.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatePostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillingRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CorpseId = table.Column<int>(type: "integer", nullable: false),
                    StorageFeePerDay = table.Column<double>(type: "double precision", nullable: false),
                    ServiceFee = table.Column<double>(type: "double precision", nullable: false),
                    TotalAmount = table.Column<double>(type: "double precision", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Corpses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cccd = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    CauseOfDeath = table.Column<string>(type: "text", nullable: false),
                    DateOfDeath = table.Column<string>(type: "text", nullable: false),
                    DateAdmitted = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StorageUnit = table.Column<string>(type: "text", nullable: true),
                    StorageSlot = table.Column<string>(type: "text", nullable: true),
                    StorageSlotId = table.Column<int>(type: "integer", nullable: true),
                    Temp = table.Column<double>(type: "double precision", nullable: true),
                    DaysStored = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    Biohazard = table.Column<int>(type: "integer", nullable: false),
                    NextOfKin_Name = table.Column<string>(type: "text", nullable: false),
                    NextOfKin_Phone = table.Column<string>(type: "text", nullable: false),
                    NextOfKin_Relationship = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    AutopsyReport_PathologistName = table.Column<string>(type: "text", nullable: true),
                    AutopsyReport_ConcludingCause = table.Column<string>(type: "text", nullable: true),
                    AutopsyReport_ToxicologyResult = table.Column<string>(type: "text", nullable: true),
                    AutopsyReport_InternalExamDetails = table.Column<string>(type: "text", nullable: true),
                    AutopsyReport_Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmbalmingInfo_IsEmbalmed = table.Column<bool>(type: "boolean", nullable: false),
                    EmbalmingInfo_EmbalmerName = table.Column<string>(type: "text", nullable: false),
                    EmbalmingInfo_ChemicalsUsed = table.Column<string>(type: "text", nullable: false),
                    EmbalmingInfo_DateEmbalmed = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corpses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StaffEmail = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShiftType = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlotNumber = table.Column<string>(type: "text", nullable: false),
                    UnitName = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentTemperature = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemperatureLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StorageSlotId = table.Column<int>(type: "integer", nullable: false),
                    Temperature = table.Column<double>(type: "double precision", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CorpseId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    DriverName = table.Column<string>(type: "text", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportTrips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LicensePlate = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportVehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Belonging",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Desc = table.Column<string>(type: "text", nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: false),
                    HasImage = table.Column<bool>(type: "boolean", nullable: false),
                    CorpseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Belonging", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Belonging_Corpses_CorpseId",
                        column: x => x.CorpseId,
                        principalTable: "Corpses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CorpseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_Corpses_CorpseId",
                        column: x => x.CorpseId,
                        principalTable: "Corpses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<string>(type: "text", nullable: false),
                    By = table.Column<string>(type: "text", nullable: false),
                    Detail = table.Column<string>(type: "text", nullable: false),
                    CorpseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryEntry_Corpses_CorpseId",
                        column: x => x.CorpseId,
                        principalTable: "Corpses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                table: "TransportVehicles",
                columns: new[] { "Id", "LicensePlate", "Model", "Status" },
                values: new object[,]
                {
                    { 1, "51B-12345", "Ford Transit Special Ambulance", 0 },
                    { 2, "51B-67890", "Mercedes-Benz Sprinter Hearse", 0 },
                    { 3, "51B-55555", "Hyundai Staria Mortuary Vehicle", 0 }
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

            migrationBuilder.CreateIndex(
                name: "IX_Belonging_CorpseId",
                table: "Belonging",
                column: "CorpseId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_CorpseId",
                table: "Document",
                column: "CorpseId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryEntry_CorpseId",
                table: "HistoryEntry",
                column: "CorpseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Belonging");

            migrationBuilder.DropTable(
                name: "BillingRecords");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "HistoryEntry");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "StorageSlots");

            migrationBuilder.DropTable(
                name: "TemperatureLogs");

            migrationBuilder.DropTable(
                name: "TransportTrips");

            migrationBuilder.DropTable(
                name: "TransportVehicles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Corpses");
        }
    }
}
