using BankManagementSystem.DAL;
using BankManagementSystem.Helpers;
using BankManagementSystem.Models;
using BankManagementSystem.Repo;
using BankManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace BankManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //private readonly ISavingsService _savingsService;
        //private readonly ICurrentService _currentService;
        private readonly AccountServiceDelegate _delegate;
        private readonly BankDbContext _context;
        private readonly ITransactionRepo _transactionRepo;


        public AccountController(AccountServiceDelegate delegates, BankDbContext context, ITransactionRepo transactionRepo)
        {
            //_savingsService = savingsService;
            //_currentService = currentService;
            _context = context;
            _delegate = delegates;
            _transactionRepo = transactionRepo;
        }

            private IActionResult ReturnOkOrBadRequest(Result result)
        {
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.DisplayAccounts);
        }

        [HttpPost("CreateAccount")]
        public IActionResult CreateAccount([FromBody] [Required] AccountDTO accountDTO)
        {
            if (accountDTO.CustomerId <= 0)
            {
                return BadRequest("Customer Id cannot be 0 or negative");
            }
            //if (accountDTO == null)
            //{
            //    return BadRequest("Invalid Data");
            //}

            //var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == accountDTO.CustomerId);
            //if (customer == null)
            //{
            //    return NotFound($"Customer with ID {accountDTO.CustomerId} not found");
            //}

            var cusIfPresent = _context.Customers.Find(accountDTO.CustomerId);
            if (cusIfPresent == null)
            {
                return BadRequest("Account creation failed.\r\nCustomer should be created first.");
            }

            // Convert the AccountType from string to enum
            if (!Enum.TryParse<AccountType>(accountDTO.AccountType, true, out var accountType))
            {
                return BadRequest("Invalid account type.");
            }

            var accountService = _delegate(accountType);

            //try
            //{
            //    IAccountService accountService = accountType switch
            //    {
            //        AccountType.Savings when accountDTO.Balance >= AccountConstants.SavingsAccountMinBalance => _savingsService,
            //        AccountType.Current when accountDTO.Balance >= AccountConstants.CurrentAccountMinBalance => _currentService,
            //        AccountType.Savings => throw new ArgumentException($"Balance must be at least {AccountConstants.SavingsAccountMinBalance} for a Savings account."),
            //        AccountType.Current => throw new ArgumentException($"Balance must be at least {AccountConstants.CurrentAccountMinBalance} for a Current account."),
            //        _ => throw new NotImplementedException() // Handle any unexpected account types
            //    };


                //// Validation based on the account type
                //if (accountType == AccountType.Savings && accountDTO.Balance < AccountConstants.SavingsAccountMinBalance)
                //{
                //    return BadRequest($"Balance must be at least {AccountConstants.SavingsAccountMinBalance} for a Savings account.");
                //}

                //if (accountType == AccountType.Current && accountDTO.Balance < AccountConstants.CurrentAccountMinBalance)
                //{
                //    return BadRequest($"Balance must be at least {AccountConstants.CurrentAccountMinBalance} for a Current account.");
                //}

                //// Determine the appropriate account service
                //IAccountService accountService = accountType switch
                //{
                //    AccountType.Savings => _savingsService,
                //    AccountType.Current => _currentService,
                //    _ => throw new NotImplementedException() // This case should ideally be unreachable due to the validation above
                //};

                // Create an instance of the account and set its properties
                //var account = new AccountDTO
                //{
                //    CustomerId = accountDTO.CustomerId,
                //    AccountType = accountType.ToString(),
                //    Balance = accountDTO.Balance
                //};

                //// Add the account using the determined service
                //accountService.AddAccount(account);


                var result = accountService.AddAccount(accountDTO);
                if (!result.Success)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(result.SuccessMsg);
                //return Ok("Account created successfully");
            //}catch(ArgumentException ex) { return BadRequest(ex.Message);}
            //catch(NotImplementedException ex) { return StatusCode(501, ex.Message); }
            //catch (Exception ex) { return StatusCode(500, "An unexpected error occured. Please try later"); }
        }


        [HttpPost("Deposit")]
        public IActionResult DepositAndAddTransaction([Required] int accId, [Required] decimal amount)
        {

            var result = _transactionRepo.DepositAndAddTransaction(accId, amount);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.SuccessMsg);
        }


        [HttpPost("Withdraw")]
        public IActionResult WithdrawAndAddTransaction([Required] int accId, [Required] decimal amount)
        {
            var result = _transactionRepo.WithdrawAndAddTransaction(accId, amount);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.SuccessMsg);
        }


        [HttpPost("Transfer Money")]
        public IActionResult TransferAndAddTransaction([Required] int senderAccountId, [Required] int receiverAccountId, [Required] int amount)
        {
            var result = _transactionRepo.TransferAndAddTransaction(senderAccountId, receiverAccountId, amount);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.SuccessMsg);
        }


        [HttpPost("Convert Account To")]
        public IActionResult ConvertAccountTo([Required] int accountId, [Required] string accountTypeStr)
        {
            if (!Enum.TryParse<AccountType>(accountTypeStr, true, out var accountType))
            {
                return BadRequest($"Invalid account type '{accountTypeStr}'. Valid values are 'Savings' or 'Current'.");
            }
            var accountService = _delegate(accountType);
            var result = accountService.ConvertAccountTo(accountId, accountType);
            if(!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.SuccessMsg);
        }



        [HttpGet("Get All Accounts")]
        public IActionResult GetAllAccounts([Required] string accountTypeStr)
        {
            if (TryGetAccountService(accountTypeStr, out var accountService, out var accountType) is IActionResult badRequestResult)
            {
                return badRequestResult;
            }

            var result = accountService.GetAllAccounts(accountType);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.DisplayAccounts);
        }



        [HttpGet("Get All Accounts Created Between")]
        public IActionResult GetAllAccountsCreatedBetween([Required] DateTime start, [Required] DateTime end,[Required] string accountTypeStr)
        {

            if (TryGetAccountService(accountTypeStr, out var accountService, out var accountType) is IActionResult badRequestResult)
            {
                return badRequestResult;
            }

            var result = accountService.GetAllAccountsCreatedBetween(start, end, accountType);
            return ReturnOkOrBadRequest(result);
        }


        
        [HttpGet("Get All Accounts Converted Between")]
        public IActionResult GetAllAccountsConvertedBetween([Required] DateTime start, [Required] DateTime end, [Required] string accountTypeStr)
        {
            // Call the helper method
            if (TryGetAccountService(accountTypeStr, out var accountService, out var accountType) is IActionResult badRequestResult)
            {
                return badRequestResult;
            }

            // Use the valid accountService and accountType
            var result = accountService.GetAllAccountsConvertedBetween(start, end, accountType);
            return ReturnOkOrBadRequest(result);
        }


        [HttpGet("Get All Accounts Having Balance Between")]
        public IActionResult GetAllAccountsHavingBalanceBetween([Required] long minBalance,[Required] long maxBalance,[Required] string accountTypeStr)
        {
            if (TryGetAccountService(accountTypeStr, out var accountService, out var accountType) is IActionResult badRequestResult)
            {
                return badRequestResult;
            }

            var result = accountService.GetAllAccountsHavingBalanceBetween(minBalance, maxBalance, accountType);
            return ReturnOkOrBadRequest(result);
        }

        
        [HttpGet("Get All Accounts Having Balance Above")]
        public IActionResult GetAllAccountsHavingBalanceAbove([Required] long balance,[Required] string accountTypeStr)
        {

            if (TryGetAccountService(accountTypeStr, out var accountService, out var accountType) is IActionResult badRequestResult)
            {
                return badRequestResult;
            }

            var result = accountService.GetAllAccountsHavingBalanceAbove(balance, accountType);
            return ReturnOkOrBadRequest(result);
        }



        [HttpGet("Get All Customers")]
        public IActionResult GetAllCustomers([Required] string accountTypeStr)
        {

            if (TryGetAccountService(accountTypeStr, out var accountService, out var accountType) is IActionResult badRequestResult)
            {
                return badRequestResult;
            }

            var result = accountService.GetAllCustomers(accountType);
            if (!result.Success)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(result.DisplayAllCustomers);
        }


        [HttpDelete("DeleteAccount")]
        public IActionResult DeleteAccount([Required] int accountId)
        {
            var accountService = _delegate(AccountType.Current);
            var result = accountService.DeleteAccount(accountId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.DisplayAllCustomers);
        }



        //helper functions
        private IActionResult TryGetAccountService(string accountTypeStr, out IAccountService accountService, out AccountType accountType)
        {
            if (!Enum.TryParse<AccountType>(accountTypeStr, true, out accountType))
            {
                accountService = null;
                return BadRequest($"Invalid account type '{accountTypeStr}'. Valid values are 'Savings', 'Current', or 'All'.");
            }

            switch (accountType)
            {
                case AccountType.All:
                    accountService = _delegate(AccountType.Savings); 
                    break;
                case AccountType.Savings:
                    accountService = _delegate(AccountType.Savings);
                    break;
                case AccountType.Current:
                    accountService = _delegate(AccountType.Current);
                    break;
                default:
                    accountService = null;
                    return BadRequest("Invalid account type");
            }

            return null; // No error, valid accountService is assigned
        }




        //[HttpGet("Get All Accounts Converted Between")]
        //public IActionResult GetAllAccountsConvertedBetween([Required] DateTime start, [Required] DateTime end,[Required] string accountTypeStr)
        //{
        //    if (!Enum.TryParse<AccountType>(accountTypeStr, true, out var accountType))
        //    {
        //        return BadRequest($"Invalid account type '{accountTypeStr}'. Valid values are 'Savings' or 'Current' or 'All'.");
        //    }
        //    IAccountService accountService;
        //    AccountType account;
        //    if (accountType == AccountType.All)
        //    {
        //        accountService = _delegate(AccountType.Savings);
        //        account = AccountType.All;
        //    }
        //    else if (accountType == AccountType.Savings)
        //    {
        //        accountService = _delegate(AccountType.Savings);
        //        account = AccountType.Savings;
        //    }
        //    else if (accountType == AccountType.Current)
        //    {
        //        accountService = _delegate(AccountType.Current);
        //        account = AccountType.Current;
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid account type");
        //    }

        //    var result = accountService.GetAllAccountsConvertedBetween(start, end, account);
        //    return ReturnOkOrBadRequest(result);
        //}


    }
}
