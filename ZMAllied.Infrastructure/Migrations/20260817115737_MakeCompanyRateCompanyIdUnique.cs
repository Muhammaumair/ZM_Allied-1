using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZMAllied.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeCompanyRateCompanyIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyRates_CompanyId",
                table: "CompanyRates");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRates_CompanyId",
                table: "CompanyRates",
                column: "CompanyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyRates_CompanyId",
                table: "CompanyRates");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRates_CompanyId",
                table: "CompanyRates",
                column: "CompanyId");
        }
    }
}
