using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPI.Migrations
{
    /// <inheritdoc />
    public partial class tm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "BuyerInfos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerInfos_UserId1",
                table: "BuyerInfos",
                column: "UserId1",
                unique: true,
                filter: "[UserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyerInfos_Users_UserId1",
                table: "BuyerInfos",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyerInfos_Users_UserId1",
                table: "BuyerInfos");

            migrationBuilder.DropIndex(
                name: "IX_BuyerInfos_UserId1",
                table: "BuyerInfos");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "BuyerInfos");
        }
    }
}
