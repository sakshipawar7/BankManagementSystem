using AutoMapper;
using Azure;
using BankManagementSystem.DAL;
using BankManagementSystem.Helpers;
using BankManagementSystem.Models;
using BankManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace BankManagementSystem.Repo
{
    //public interface ICurrentService : IAccountService
    //{
    //    //public void AddAccount(AccountDTO accountDto);

    //}
    public class CurrentAccountService : Accounts
    {   
        private readonly BankDbContext _context;
        private readonly IMapper _mapper;
        

        public CurrentAccountService(BankDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            UpdateDailyLimitsForAllAccounts();
        }

        public override void UpdateDailyLimitsForAllAccounts()
        {
            var accounts = _context.Accounts.ToList();

            foreach (var account in accounts)
            {
                CheckLastReset.Check(account);
            }

            _context.SaveChanges();
        }

        public override Result AddAccount(AccountDTO accountDto)
        {            
            if (accountDto.Balance < AccountConstants.CurrentAccountMinBalance)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Minimum balance for current account should be {AccountConstants.CurrentAccountMinBalance}"
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
                RemainingWithdrawlAmountPerDay = AccountConstants.MaxWithdrawlAmountPerDayCurrent,
                RemainingNoOfWithdrawlsPerDay = AccountConstants.MaxNoOfWithdrawlsPerDayCurrent,
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
                SuccessMsg = $"Account created successfully.\r\nYour account Id is {account.AccountId}"
            };
        }

        

        
        public override Result ConvertAccountTo(int accId, AccountType accountType)
        {
            if(accId <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Account Id cannot be 0 or negative"
                };
            }

            var initialAcc = _context.Accounts.FirstOrDefault(a => a.AccountId == accId && !a.IsDeleted);

            if(initialAcc == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No account found with account id {accId}"
                };
            }

            if(initialAcc.AccountType == accountType)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Conversion failed.\r\nPrevious account type and this account type are same."
                };
            }

            if(initialAcc.Balance < AccountConstants.CurrentAccountMinBalance)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Conversion failed.\r\n" +
                    $"For converting account to Current Account the minimum balance should be {AccountConstants.CurrentAccountMinBalance}" +
                    $"\r\nPlease deposit {AccountConstants.CurrentAccountMinBalance - initialAcc.Balance} rupees and try again"
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



        //public Result UpdateAccountByAccountType(AccountType accountType, JsonPatchDocument<UpdateConstantsCurrentModel> patch)
        //{

        //}




        //public override Result GetAllAccounts()
        //{   
        //    var dalAccounts = _context.Accounts.Where(a=>a.AccountType == AccountType.Current).ToList();
        //    if(dalAccounts.Count == 0)
        //    {
        //        return new Result
        //        {
        //            Success = false,
        //            ErrorMessage = $"No accounts found of type {AccountType.Current}",
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


        //public override Result GetAllCustomersOfAccountType(AccountType accountType)
        //{
        //                var dalCustomers = _context.Accounts
        //        .Where(a=> a.AccountType == accountType && !a.IsDeleted)
        //        .Select(a=>a.Customer)
        //        .Distinct()
        //        .Where(c=> !c.IsDeleted)
        //        .ToList();

        //    if(dalCustomers.Count == 0)
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

