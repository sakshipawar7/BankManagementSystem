using AutoMapper;
using BankManagementSystem.DAL;
using BankManagementSystem.Helpers;
using BankManagementSystem.Models;
using BankManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Repo
{
    abstract public class Accounts : IAccountService
    {

        private readonly BankDbContext _context;
        private readonly IMapper _mapper;

        public Accounts(BankDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public abstract Result AddAccount(AccountDTO accountDto);
        public abstract Result ConvertAccountTo(int accId, AccountType accountType);
        public abstract void UpdateDailyLimitsForAllAccounts();


        
        public Result GetAllAccounts(AccountType accountType)
        {
            List<Account> dalAccounts;

            if (accountType == AccountType.All)
            {
                dalAccounts = _context.Accounts.Where(a=>!a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Savings)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Savings && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Current)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Current && !a.IsDeleted).ToList();
            }
            else
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid account type specified"
                };
            }

            if (dalAccounts.Count == 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "No Accounts in database"
                };
            }

            var accounts = _mapper.Map<List<DisplayAccountsModel>>(dalAccounts);

            return new Result
            {
                Success = true,
                DisplayAccounts = accounts
            };
        }

        public Result GetAllCustomers(AccountType accountType)
        {
            List<Customer> dalCustomers;
            if (accountType == AccountType.All)
            {
                dalCustomers = _context.Accounts
                .Where(a => !a.IsDeleted)
                .Select(a => a.Customer)
                .Distinct()
                .Where(c => !c.IsDeleted)
                .ToList();
            }
            else if (accountType == AccountType.Savings || accountType == AccountType.Current)
            {
                dalCustomers = _context.Accounts
                .Where(a => a.AccountType == accountType && !a.IsDeleted)
                .Select(a => a.Customer)
                .Distinct()
                .Where(c => !c.IsDeleted)
                .ToList();
            }
            else
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid account type specified"
                };
            }

            if (dalCustomers.Count == 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No customer found of account type {accountType}"
                };
            }

            var customers = _mapper.Map<List<DisplayCustomerModel>>(dalCustomers);
            return new Result
            {
                Success = true,
                DisplayAllCustomers = customers
            };
        }




        public Result GetAllAccountsCreatedBetween(DateTime startDate, DateTime endDate, AccountType accountType)
        {
            var result = ValidateStartAndEndDate(startDate, endDate);
            if (!result.Success)
            {
                return result;
            }

            List<Account> dalAccounts;

            if (accountType == AccountType.All)
            {
                dalAccounts = _context.Accounts.Where(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Savings)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Savings && a.CreatedDate >= startDate && a.CreatedDate <= endDate && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Current)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Current && a.CreatedDate >= startDate && a.CreatedDate <= endDate && !a.IsDeleted).ToList();
            }
            else
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid account type specified"
                };
            }

            result = CheckCount(dalAccounts, startDate, endDate);
            if (!result.Success)
            {
                return result;
            }

            var accounts = _mapper.Map<List<DisplayAccountsModel>>(dalAccounts);
            return new Result
            {
                Success = true,
                DisplayAccounts = accounts
            };
        }


        public Result GetAllAccountsConvertedBetween(DateTime startDate, DateTime endDate, AccountType accountType)
        {
            var result = ValidateStartAndEndDate(startDate, endDate);
            if (!result.Success)
            {
                return result;
            }

            List<Account> dalAccounts;

            if (accountType == AccountType.All)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountConvertedDate >= startDate && a.AccountConvertedDate <= endDate && a.AccountConvertedDate != DateTime.MinValue && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Savings)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Savings && a.AccountConvertedDate >= startDate && a.AccountConvertedDate <= endDate && a.AccountConvertedDate != DateTime.MinValue && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Current)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Current && a.AccountConvertedDate >= startDate && a.AccountConvertedDate <= endDate && a.AccountConvertedDate != DateTime.MinValue && !a.IsDeleted).ToList();
            }
            else
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid account type specified"
                };
            }

            result = CheckCount(dalAccounts, startDate, endDate);
            if (!result.Success)
            {
                return result;
            }

            var accounts = _mapper.Map<List<DisplayAccountsModel>>(dalAccounts);
            return new Result
            {
                Success = true,
                DisplayAccounts = accounts
            };
        }


        public Result GetAllAccountsHavingBalanceBetween(long minBalance, long maxBalance, AccountType accountType)
        {
            if (minBalance < 0 || maxBalance < 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Balance should not be negative"
                };
            }

            if (minBalance > maxBalance)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "MinBalance should be less than MaxBalance"
                };
            }

            List<Account> dalAccounts;

            if (accountType == AccountType.All)
            {
                dalAccounts = _context.Accounts.Where(a => a.Balance >= minBalance && a.Balance <= maxBalance && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Savings)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Savings && a.Balance >= minBalance && a.Balance <= maxBalance && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Current)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Current && a.Balance >= minBalance && a.Balance <= maxBalance && !a.IsDeleted).ToList();
            }
            else
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid account type specified"
                };
            }

            if (dalAccounts.Count == 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No accounts found having balance between {minBalance} and {maxBalance}"
                };
            }

            var accounts = _mapper.Map<List<DisplayAccountsModel>>(dalAccounts);
            return new Result
            {
                Success = true,
                DisplayAccounts = accounts
            };
        }


        public Result GetAllAccountsHavingBalanceAbove(long balance, AccountType accountType)
        {
            if (balance < 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Balance cannot be negative"
                };
            }

            List<Account> dalAccounts;

            if (accountType == AccountType.All)
            {
                dalAccounts = _context.Accounts.Where(a => a.Balance >= balance && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Savings)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Savings && a.Balance >= balance && !a.IsDeleted).ToList();
            }
            else if (accountType == AccountType.Current)
            {
                dalAccounts = _context.Accounts.Where(a => a.AccountType == AccountType.Current && a.Balance >= balance && !a.IsDeleted).ToList();
            }
            else
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid account type specified"
                };
            }

            if (dalAccounts.Count == 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No account found above balance {balance}"
                };
            }

            var accounts = _mapper.Map<List<DisplayAccountsModel>>(dalAccounts);
            return new Result
            {
                Success = true,
                DisplayAccounts = accounts
            };
        }


        //helper functions
        public Result ValidateStartAndEndDate(DateTime startDate, DateTime endDate)
        {
            if (startDate > DateTime.Now)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Start date should be before date-time {DateTime.Now}"
                };
            }

            if (startDate > endDate)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Start date should be before End date"
                };
            }

            if(startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "No accounts found"
                };
            }
            return new Result
            {
                Success = true
            };
        }



        public Result CheckCount(List<Account> dalAccounts, DateTime startDate, DateTime endDate)
        {
            if (dalAccounts.Count == 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No accounts found betwwen daterange {startDate.ToString("yyyy-MM-dd HH:mm:ss")} and {endDate.ToString("yyyy-MM-dd HH:mm:ss")}"
                };
            }
            return new Result
            {
                Success = true
            };
        }

        public Result DeleteAccount(int accId)
        {
            if (accId <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Account Id cannot be 0 or negative"
                };
            }

            var account = _context.Accounts.FirstOrDefault(x => x.AccountId == accId && !x.IsDeleted);
            if (account == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "No account found"
                };
            }

            var customer = _context.Accounts
                .Where(a => a.AccountId == accId)
                .Select(a => a.Customer)
                .FirstOrDefault();

            int accountCount = _context.Accounts
                .Count(a => a.CustomerId == customer.CustomerId);

            account.IsDeleted = true;
            _context.SaveChanges();

            var transactions = _context.Transactions
                .Where(t => (t.SenderAccountId == accId && t.ReceiverAccountId == accId)
                && (t.TransactionType == TransactionType.Withdraw || t.TransactionType == TransactionType.Deposit))
                .ToList();

            foreach (var transaction in transactions)
            {
                transaction.IsDeleted = true;
                _context.SaveChanges();
            }

            if (accountCount <= 1)
            {
                customer.IsDeleted = true;
                _context.SaveChanges();

            }
            return new Result
            {
                Success = true,
                SuccessMsg = $"Account with account id {accId} deleted successfully."
            };
        }


    }
}
