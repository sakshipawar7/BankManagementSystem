namespace BankManagementSystem.Models
{
    public class UpdateConstantsSavingsModel
    {
        public decimal CurrentAccountInterestRate { get; set; }
        public decimal CurrentAccountMinBalance { get; set; }
        public byte MaxNoOfWithdrawlsPerDaySavings { get; set; }
        public int MaxWithdrawlAmountPerDaySavings { get; set; }
    }
}
