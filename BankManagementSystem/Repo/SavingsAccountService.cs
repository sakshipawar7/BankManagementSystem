using BankManagementSystem.DAL;
using BankManagementSystem.Helpers;
using BankManagementSystem.Services;
using AutoMapper;
using BankManagementSystem.Models;
using System.Collections;
using Microsoft.EntityFrameworkCore;
namespace BankManagementSystem.Repo
{
    //public interface ISavingsService : IAccountService
    //{
    //    //public void AddAccount(AccountDTO accountDto);
    //}
    public class SavingsAccountService : Accounts
    {
        private readonly BankDbContext _context;
        private readonly IMapper _mapper;

        public SavingsAccountService(BankDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;            
            UpdateDailyLimitsForAllAccounts();
        }

        public override void UpdateDailyLimitsForAllAccounts()
        {
            //DateTime utcNow = DateTime.UtcNow;
            //TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, istTimeZone);
            // Fetch all accounts from the database
            var accounts = _context.Accounts.ToList();

            foreach (var account in accounts)
            {
                CheckLastReset.Check(account);
                // Check if more than 24 hours have passed since last reset
                //if (istNow - account.LastResetDate > TimeSpan.FromDays(1))
                //{
                //    account.RemainingWithdrawlAmountPerDay = (account.AccountType == AccountType.Current) ? AccountConstants.MaxWithdrawlAmountPerDayCurrent : AccountConstants.MaxWithdrawlAmountPerDaySavings;
                //    account.RemainingNoOfWithdrawlsPerDay = (account.AccountType == AccountType.Current) ? AccountConstants.MaxNoOfWithdrawlsPerDayCurrent : AccountConstants.MaxNoOfWithdrawlsPerDaySavings;
                //    account.LastResetDate = DateTime.UtcNow;
                //}
            }

            // Save changes to the database
            _context.SaveChanges();
        }


        public override Result AddAccount(AccountDTO accountDto)
        {            
            if (accountDto.Balance < AccountConstants.SavingsAccountMinBalance)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Minimum balance for current account should be {AccountConstants.SavingsAccountMinBalance}"
                };
            }
            if (!Enum.TryParse<AccountType>(accountDto.AccountType, true, out var accountType))
            {
                throw new ArgumentException("Invalid account type.", nameof(accountDto.AccountType));
            }
            var account = new Account
            {
                CustomerId = accountDto.CustomerId,
                AccountType = accountType,
                Balance = accountDto.Balance,
                CreatedDate = DateTime.Now,
                AccountConvertedDate = DateTime.Now,
                RemainingWithdrawlAmountPerDay = AccountConstants.MaxWithdrawlAmountPerDaySavings,
                RemainingNoOfWithdrawlsPerDay = AccountConstants.MaxNoOfWithdrawlsPerDaySavings
            };

            _context.Accounts.Add(account);

            var transaction = new Transaction
            {
                SenderAccountId = account.AccountId,
                ReceiverAccountId = account.AccountId,
                TransactionType = TransactionType.Deposit,
                Amount = accountDto.Balance,
                TCreationDate = DateTime.Now,
                Description = $"Deposited {TransactionRepo.ConvertDollarSymbolToRupeeSymbol(accountDto.Balance)} to account id {account.AccountId}"
            };

            _context.SaveChanges();
            return new Result
            {
                Success = true,
                SuccessMsg = $"Account created successfully.\r\n Your account Id is {account.AccountId}"
            };
        }

        public override Result ConvertAccountTo(int accId, AccountType accountType)
        {
            if (accId <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Account Id cannot be 0 or negative"
                };
            }

            var initialAcc = _context.Accounts.FirstOrDefault(a => a.AccountId == accId && !a.IsDeleted);

            if (initialAcc == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No account found with account id {accId}"
                };
            }

            if (initialAcc.AccountType == accountType)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Conversion failed.\r\nPrevious account type and this account type are same."
                };
            }

            if (initialAcc.Balance < AccountConstants.SavingsAccountMinBalance)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Conversion failed.\r\n" +
                    $"For converting account to Current Account the minimum balance should be {AccountConstants.SavingsAccountMinBalance}" +
                    $"\r\nPlease deposit {AccountConstants.SavingsAccountMinBalance - initialAcc.Balance} rupees and try again"
                };
            }

            initialAcc.AccountType = accountType;
            initialAcc.AccountConvertedDate = DateTime.Now;
            _context.Accounts.Update(initialAcc);
            _context.SaveChanges();

            return new Result
            {
                Success = true,
                SuccessMsg = $"Account converted successfully to Account type {initialAcc.AccountType}"
            };

        }


        //public override Result GetAllAccounts()
        //{
        //    var dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Savings).ToList();
        //    if (dalAccounts.Count == 0)
        //    {
        //        return new Result
        //        {
        //            Success = false,
        //            ErrorMessage = $"No accounts found of type {AccountType.Savings}",
        //            DisplayAccounts = new List<DisplayAccounts>()
        //        };
        //    }

        //    var accounts = _mapper.Map<List<DisplayAccounts>>(dalAccounts);
        //    return new Result
        //    {
        //        Success = true,
        //        DisplayAccounts = accounts
        //    };
        //}


        //public override  Result GetAllCustomersOfAccountType(AccountType accountType)
        //{
        //    var dalCustomers = _context.Accounts
        //        .Where(a => a.AccountType == accountType && !a.IsDeleted)
        //        .Select(a => a.Customer)
        //        .Distinct()
        //        .Where(c => !c.IsDeleted)
        //        .ToList();

        //    if (dalCustomers.Count == 0)
        //    {
        //        return new Result
        //        {
        //            Success = false,
        //            ErrorMessage = $"No customer found of account type {accountType}"
        //        };
        //    }

        //    var customers = _mapper.Map<List<DisplayCustomerModel>>(dalCustomers);
        //    return new Result
        //    {
        //        Success = true,
        //        DisplayAllCustomers = customers
        //    };
        //}
    }
}
