using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPI.Migrations
{
    /// <inheritdoc />
    public partial class sm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "Transactions",
                newName: "ShipingStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShipingStatus",
                table: "Transactions",
                newName: "OrderStatus");
        }
    }
}
