using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedTransactionAndSmallUpdatesMade2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Accounts_ReceiverAccountId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Accounts_SenderAccountId",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_SenderAccountId",
                table: "Transactions",
                newName: "IX_Transactions_SenderAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_ReceiverAccountId",
                table: "Transactions",
                newName: "IX_Transactions_ReceiverAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_ReceiverAccountId",
                table: "Transactions",
                column: "ReceiverAccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId",
                table: "Transactions",
                column: "SenderAccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_ReceiverAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transaction");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_SenderAccountId",
                table: "Transaction",
                newName: "IX_Transaction_SenderAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_ReceiverAccountId",
                table: "Transaction",
                newName: "IX_Transaction_ReceiverAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Accounts_ReceiverAccountId",
                table: "Transaction",
                column: "ReceiverAccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Accounts_SenderAccountId",
                table: "Transaction",
                column: "SenderAccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId");
        }
    }
}
