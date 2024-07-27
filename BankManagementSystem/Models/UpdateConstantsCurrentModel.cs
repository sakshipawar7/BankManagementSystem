namespace BankManagementSystem.Models
{
    public class UpdateConstantsCurrentModel
    {
        public decimal SavingsAccountInterestRate { get; set; }
        public decimal SavingsAccountMinBalance { get; set; }
        public byte MaxNoOfWithdrawlsPerDayCurrent { get; set; }
        public int MaxWithdrawlAmountPerDayCurrent { get; set; }
    }
}
