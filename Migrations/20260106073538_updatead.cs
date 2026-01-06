using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mini_Social_Media.Migrations
{
    /// <inheritdoc />
    public partial class updatead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Advertisements",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Advertisements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdvertisementId1",
                table: "AdStats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_UserId1",
                table: "Advertisements",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AdStats_AdvertisementId1",
                table: "AdStats",
                column: "AdvertisementId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AdStats_Advertisements_AdvertisementId1",
                table: "AdStats",
                column: "AdvertisementId1",
                principalTable: "Advertisements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisements_AspNetUsers_UserId1",
                table: "Advertisements",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdStats_Advertisements_AdvertisementId1",
                table: "AdStats");

            migrationBuilder.DropForeignKey(
                name: "FK_Advertisements_AspNetUsers_UserId1",
                table: "Advertisements");

            migrationBuilder.DropIndex(
                name: "IX_Advertisements_UserId1",
                table: "Advertisements");

            migrationBuilder.DropIndex(
                name: "IX_AdStats_AdvertisementId1",
                table: "AdStats");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "AdvertisementId1",
                table: "AdStats");
        }
    }
}
