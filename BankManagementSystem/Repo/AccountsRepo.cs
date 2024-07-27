//using AutoMapper;
//using BankManagementSystem.DAL;
//using BankManagementSystem.Helpers;
//using BankManagementSystem.Models;
//using BankManagementSystem.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace BankManagementSystem.Repo
//{
//    public interface IAccountsRepo //used by AccountsRepo
//    {

//    }

    
//    public class AccountsRepo : IAccountsRepo
//    {
//        private IAccountSavingsOrCurrent _accountSavingsOrCurrent;
//        private readonly BankDbContext _context;
//        private readonly IMapper _mapper;
//        private readonly AccountServiceDelegate2 _delegate;
//        public AccountsRepo(IAccountSavingsOrCurrent accountSavingsOrCurrent, BankDbContext context, IMapper mapper, AccountServiceDelegate2 delegates)
//        {
//            _accountSavingsOrCurrent = accountSavingsOrCurrent;
//            _context = context;
//            _mapper = mapper;
//            _delegate = delegates;
//        }

//        public Result GetAllAccounts(AccountType accountType)
//        {

//            List<Account> dalAccounts;

//            if (accountType == AccountType.All)
//            {
//                dalAccounts = _context.Accounts.Where(a => !a.IsDeleted).ToList();
//            }
//            else if (accountType == AccountType.Savings)
//            {
//                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Savings && !a.IsDeleted).ToList();
//            }
//            else if (accountType == AccountType.Current)
//            {
//                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Current && !a.IsDeleted).ToList();
//            }
//            else
//            {
//                return new Result
//                {
//                    Success = false,
//                    ErrorMessage = "Invalid account type specified"
//                };
//            }

//            if (dalAccounts.Count == 0)
//            {
//                return new Result
//                {
//                    Success = false,
//                    ErrorMessage = "No Accounts in database"
//                };
//            }

//            var accounts = _mapper.Map<List<DisplayAccountsModel>>(dalAccounts);

//            return new Result
//            {
//                Success = true,
//                DisplayAccounts = accounts
//            };
//        }


//        //helper functions
//        private IActionResult TryGetAccountService(string accountTypeStr, out IAccountService accountService, out AccountType accountType)
//        {
//            if (!Enum.TryParse<AccountType>(accountTypeStr, true, out accountType))
//            {
//                accountService = null;
//                return BadRequest($"Invalid account type '{accountTypeStr}'. Valid values are 'Savings', 'Current', or 'All'.");
//            }

//            switch (accountType)
//            {
//                case AccountType.All:
//                    accountService = _delegate(AccountType.Savings);
//                    break;
//                case AccountType.Savings:
//                    accountService = _delegate(AccountType.Savings);
//                    break;
//                case AccountType.Current:
//                    accountService = _delegate(AccountType.Current);
//                    break;
//                default:
//                    accountService = null;
//                    return BadRequest("Invalid account type");
//            }

//            return null; // No error, valid accountService is assigned
//        }
//    }

//    public interface IAccountSavingsOrCurrent //used by savings,current
//    {

//    }

//    public class Savings : IAccountSavingsOrCurrent
//    {

//    }
//    public class Current : IAccountSavingsOrCurrent
//    {

//    }


    
//}
