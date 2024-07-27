using BankManagementSystem.DAL;
using BankManagementSystem.Helpers;
using BankManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Services
{
    public interface IAccountService
    {
        
        public Result AddAccount(AccountDTO accountDto);        
        public Result ConvertAccountTo(int accId, AccountType accountType);
        public Result DeleteAccount(int accId);



        //common functions defined in Accounts.cs.
        public Result GetAllAccounts(AccountType accountType);
        public Result GetAllCustomers(AccountType accountType);
        public Result GetAllAccountsCreatedBetween(DateTime startDate, DateTime endDate, AccountType accountType);


        public Result GetAllAccountsConvertedBetween(DateTime startDate, DateTime endDate, AccountType accountType);
        public Result GetAllAccountsHavingBalanceBetween(long minBalance, long maxBalance, AccountType accountType);
        public Result GetAllAccountsHavingBalanceAbove(long balance, AccountType accountType);


        public void UpdateDailyLimitsForAllAccounts();
        
    }  
}
