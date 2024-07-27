using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenamedMaxWithdrawlAndMaxNoOfWithdrawl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxWithdrawlAmountPerDay",
                table: "Accounts",
                newName: "RemainingWithdrawlAmountPerDay");

            migrationBuilder.RenameColumn(
                name: "MaxNoOfWithdrawlPerDay",
                table: "Accounts",
                newName: "RemainingNoOfWithdrawlsPerDay");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RemainingWithdrawlAmountPerDay",
                table: "Accounts",
                newName: "MaxWithdrawlAmountPerDay");

            migrationBuilder.RenameColumn(
                name: "RemainingNoOfWithdrawlsPerDay",
                table: "Accounts",
                newName: "MaxNoOfWithdrawlPerDay");
        }
    }
}
