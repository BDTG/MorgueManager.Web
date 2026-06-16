using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MorgueManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbalmingInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmbalmingInfo_ChemicalsUsed",
                table: "Corpses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmbalmingInfo_DateEmbalmed",
                table: "Corpses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmbalmingInfo_EmbalmerName",
                table: "Corpses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EmbalmingInfo_IsEmbalmed",
                table: "Corpses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmbalmingInfo_ChemicalsUsed",
                table: "Corpses");

            migrationBuilder.DropColumn(
                name: "EmbalmingInfo_DateEmbalmed",
                table: "Corpses");

            migrationBuilder.DropColumn(
                name: "EmbalmingInfo_EmbalmerName",
                table: "Corpses");

            migrationBuilder.DropColumn(
                name: "EmbalmingInfo_IsEmbalmed",
                table: "Corpses");
        }
    }
}
