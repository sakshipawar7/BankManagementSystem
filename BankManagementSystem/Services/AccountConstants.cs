namespace BankManagementSystem.Services
{
    public class AccountConstants
    {
        public const decimal CurrentAccountInterestRate = 0.03m;
        public const decimal CurrentAccountMinBalance = 1000m;
        public const byte MaxNoOfWithdrawlsPerDaySavings = 4;
        public const int MaxWithdrawlAmountPerDaySavings = 1000;

        public const decimal SavingsAccountInterestRate = 0.05m;
        public const decimal SavingsAccountMinBalance = 500m;
        public const byte MaxNoOfWithdrawlsPerDayCurrent = 5;
        public const int MaxWithdrawlAmountPerDayCurrent = 2000;
    }
}
