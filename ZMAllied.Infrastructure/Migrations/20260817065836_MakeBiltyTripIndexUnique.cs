using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZMAllied.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeBiltyTripIndexUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bilties_TripId",
                table: "Bilties");

            migrationBuilder.CreateIndex(
                name: "IX_Bilties_TripId",
                table: "Bilties",
                column: "TripId",
                unique: true,
                filter: "[TripId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bilties_TripId",
                table: "Bilties");

            migrationBuilder.CreateIndex(
                name: "IX_Bilties_TripId",
                table: "Bilties",
                column: "TripId",
                filter: "[TripId] IS NOT NULL");
        }
    }
}
