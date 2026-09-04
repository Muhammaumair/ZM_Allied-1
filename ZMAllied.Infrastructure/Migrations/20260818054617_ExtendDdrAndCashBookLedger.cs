using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZMAllied.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtendDdrAndCashBookLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Item",
                table: "DDRs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "DDRs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartyId",
                table: "DDRs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rent",
                table: "DDRs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "CashBookEntries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "CashBookEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptId",
                table: "CashBookEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "CashBookEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql(@"
                ;WITH Ledger AS
                (
                    SELECT [Id],
                        SUM([Debit] - [Credit]) OVER
                        (ORDER BY [Date], [Id] ROWS UNBOUNDED PRECEDING) AS [CalculatedBalance]
                    FROM [CashBookEntries]
                    WHERE [IsDeleted] = 0
                )
                UPDATE [CashBookEntries]
                SET [Balance] = Ledger.[CalculatedBalance]
                FROM [CashBookEntries]
                INNER JOIN Ledger ON Ledger.[Id] = [CashBookEntries].[Id];");

            migrationBuilder.CreateIndex(
                name: "IX_DDRs_PartyId",
                table: "DDRs",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashBookEntries_Date",
                table: "CashBookEntries",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CashBookEntries_PaymentId",
                table: "CashBookEntries",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CashBookEntries_ReceiptId",
                table: "CashBookEntries",
                column: "ReceiptId",
                unique: true,
                filter: "[ReceiptId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CashBookEntries_ReferenceNumber",
                table: "CashBookEntries",
                column: "ReferenceNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_CashBookEntries_Payments_PaymentId",
                table: "CashBookEntries",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CashBookEntries_Receipts_ReceiptId",
                table: "CashBookEntries",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DDRs_Parties_PartyId",
                table: "DDRs",
                column: "PartyId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashBookEntries_Payments_PaymentId",
                table: "CashBookEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_CashBookEntries_Receipts_ReceiptId",
                table: "CashBookEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_DDRs_Parties_PartyId",
                table: "DDRs");

            migrationBuilder.DropIndex(
                name: "IX_DDRs_PartyId",
                table: "DDRs");

            migrationBuilder.DropIndex(
                name: "IX_CashBookEntries_Date",
                table: "CashBookEntries");

            migrationBuilder.DropIndex(
                name: "IX_CashBookEntries_PaymentId",
                table: "CashBookEntries");

            migrationBuilder.DropIndex(
                name: "IX_CashBookEntries_ReceiptId",
                table: "CashBookEntries");

            migrationBuilder.DropIndex(
                name: "IX_CashBookEntries_ReferenceNumber",
                table: "CashBookEntries");

            migrationBuilder.DropColumn(
                name: "Item",
                table: "DDRs");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "DDRs");

            migrationBuilder.DropColumn(
                name: "PartyId",
                table: "DDRs");

            migrationBuilder.DropColumn(
                name: "Rent",
                table: "DDRs");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "CashBookEntries");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "CashBookEntries");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "CashBookEntries");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "CashBookEntries");
        }
    }
}
