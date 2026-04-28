using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradieMateWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGSTRateAndIsPro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GSTRate",
                table: "BusinessSettings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPro",
                table: "BusinessSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "BusinessSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GSTRate", "IsPro" },
                values: new object[] { 10.0, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GSTRate",
                table: "BusinessSettings");

            migrationBuilder.DropColumn(
                name: "IsPro",
                table: "BusinessSettings");
        }
    }
}
