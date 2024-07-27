using BankManagementSystem.DAL;
using BankManagementSystem.Services;
using BankManagementSystem.Models;
using BankManagementSystem.Migrations;

namespace BankManagementSystem.Helpers
{
    public class CheckLastReset
    {
        public static void Check(Account account)
        {
            //DateTime utcNow = DateTime.UtcNow;
            //TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, istTimeZone);

            //if(DateTime.Now - account.AccountConvertedDate > TimeSpan.FromDays(1))
            //{
            //    account.RemainingWithdrawlAmountPerDay = (account.AccountType == AccountType.Current) ? AccountConstants.MaxWithdrawlAmountPerDayCurrent : AccountConstants.MaxWithdrawlAmountPerDaySavings;
            //    account.RemainingNoOfWithdrawlsPerDay = (account.AccountType == AccountType.Current) ? AccountConstants.MaxNoOfWithdrawlsPerDayCurrent : AccountConstants.MaxNoOfWithdrawlsPerDaySavings;
            //    account.LastResetDate = DateTime.Now;
            //}

            if (DateTime.Now - account.LastResetDate > TimeSpan.FromDays(1))
            {
                account.RemainingWithdrawlAmountPerDay = (account.AccountType == AccountType.Current) ? AccountConstants.MaxWithdrawlAmountPerDayCurrent : AccountConstants.MaxWithdrawlAmountPerDaySavings;
                account.RemainingNoOfWithdrawlsPerDay = (account.AccountType == AccountType.Current) ? AccountConstants.MaxNoOfWithdrawlsPerDayCurrent : AccountConstants.MaxNoOfWithdrawlsPerDaySavings;
                account.LastResetDate = DateTime.Now;
            }
        }
    }
}
