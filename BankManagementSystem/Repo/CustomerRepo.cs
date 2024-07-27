using BankManagementSystem.DAL;
using BankManagementSystem.Models;
using AutoMapper;
using BankManagementSystem.Helpers;
using BankManagementSystem.Services;
using Microsoft.AspNetCore.JsonPatch;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
namespace BankManagementSystem.Repo
{
    public interface ICustomerRepo
    {
        public Result CreateCustomer(CustomerAccountHybridModel c);
        public Result UpdateCustomer(int id, JsonPatchDocument<UpdateCustomerModel> patchDoc);
        public Result GetCustomerDetailsById(int cid);
        public Result DisplayAllCustomers();
        public Result GetAllCustomersByBranch(string branch);
        public Result GetAllAccountsOfCustomerId(int cid);
        public Result DeleteCustomer(int cid);
    }
    public class CustomerRepo : ICustomerRepo
    {
        private readonly BankDbContext _context;
        private readonly IMapper _mapper;
        //private readonly ISavingsService _savingsService;
        //private readonly ICurrentService _currentService;
        private readonly AccountServiceDelegate _delegate;
        private readonly ILogger<CustomerRepo> _logger;

        public CustomerRepo(BankDbContext context, IMapper mapper, AccountServiceDelegate delegates, ILogger<CustomerRepo> logger)
        {
            _context = context;
            _mapper = mapper;
            //_savingsService = savingsService;
            //_currentService = currentService;
            _delegate = delegates;
            _logger = logger;
        }

        
        public Result CreateCustomer(CustomerAccountHybridModel c)
        {
            _logger.LogInformation("Creating customer");
            var cus = _mapper.Map<Customer>(c);


            var existingCustomer = _context.Customers.FirstOrDefault(cus => cus.AadharNo == c.AadharNo && !cus.IsDeleted);

            if (existingCustomer != null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Customer with the same AadharNo {c.AadharNo} already exists."
                };
            }

            if (!Enum.TryParse<AccountType>(c.AccountType, out var accountType))
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid account type."
                };
            }

            if (accountType == AccountType.Savings && c.Balance < AccountConstants.SavingsAccountMinBalance)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Balance must be at least {AccountConstants.SavingsAccountMinBalance} for a Savings account."
                };
            }
            if (accountType == AccountType.Current && c.Balance < AccountConstants.CurrentAccountMinBalance)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Balance must be at least {AccountConstants.CurrentAccountMinBalance} for a Current account."
                };
            }

            _context.Customers.Add(cus);
            _context.SaveChanges();

            var accountDto = new AccountDTO
            {
                AccountType = accountType.ToString(),
                Balance = c.Balance,
                CustomerId = cus.CustomerId
            };

            _logger.LogInformation($"Creating account of type {accountDto.AccountType}");

            var accountService = _delegate(accountType);

            accountService.AddAccount(accountDto);
            _logger.LogInformation("Account created successfully");

            return new Result
            {
                Success = true,
                SuccessMsg = $"Customer created successfully\r\nYour customer id is {cus.CustomerId}"
            };

        }


        public Result UpdateCustomer(int id, JsonPatchDocument<UpdateCustomerModel> patchDoc)
        {
            if (id <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Invalid id {id}"
                };
            }
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Customer not found."
                };
            }

            var updateCustomerModel = _mapper.Map<UpdateCustomerModel>(customer);

            try
            {
                patchDoc.ApplyTo(updateCustomerModel);

                var validationContext = new ValidationContext(updateCustomerModel);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(updateCustomerModel, validationContext, validationResults, validateAllProperties: true);

                if (!isValid)
                {
                    var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = "Validation failed.",
                        ValidationErrors = errors
                    };
                }

                _mapper.Map(updateCustomerModel, customer);
                customer.CustomerDetailsUpdateDate = DateTime.Now;
                _context.SaveChanges();

                _logger.LogInformation($"Customer with ID {id} updated successfully.");

                return new Result
                {
                    Success = true,
                    Data = updateCustomerModel,
                    SuccessMsg = "Customer details updated successfully"
                };
            }
            catch (JsonPatchException ex)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Invalid JsonPatch operation: {ex.Message}"
                };
            }
        }

        public Result DeleteCustomer(int cid)
        {
            if(cid <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Customer id cannot be negative or 0."
                };
            }

            var cus = _context.Customers.FirstOrDefault(c=>c.CustomerId == cid && !c.IsDeleted);    
            if(cus == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No customer found of customer id {cid}"
                };
            }

            cus.IsDeleted = true;
            _context.SaveChanges();

            var accounts = _context.Accounts.Where(a=>a.CustomerId == cid && !a.IsDeleted).ToList();
            foreach(var account in accounts)
            {
                account.IsDeleted = true;
                
                var transactions = _context.Transactions
                .Where(t => (t.SenderAccountId == account.AccountId || t.ReceiverAccountId == account.AccountId)
                && (t.TransactionType == TransactionType.Withdraw || t.TransactionType == TransactionType.Deposit)
                && !t.IsDeleted)
                .ToList();

                foreach (var transaction in transactions)
                {
                    transaction.IsDeleted = true;
                }

                var transferTransactions = _context.Transactions
                    .Where(t=> (t.SenderAccountId ==  cid && !t.IsDeleted) && t.TransactionType == TransactionType.Transfer && t.ReceiverAccount.IsDeleted)
                    .ToList();
                foreach(var transaction in transferTransactions)
                {
                    transaction.IsDeleted = true;
                }
            }

            _context.SaveChanges();
            return new Result
            {
                Success = true,
                SuccessMsg = "Customer deleted successfully."
            };           

        }


        public Result GetCustomerDetailsById(int cid)
        {
            if (cid <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Invalid Customer Id {cid}"
                };
            }

            var dalCus = _context.Customers.FirstOrDefault(c => c.CustomerId == cid && !c.IsDeleted);


            if (dalCus == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Customer with Id {cid} not found"
                };
            }

            var cus = _mapper.Map<DisplayCustomerModel>(dalCus);

            return new Result
            {
                Success = true,
                DisplayCustomer = cus
            };
        }


        public Result DisplayAllCustomers()
        {
            var dalCusts = _context.Customers.Where(a => !a.IsDeleted).ToList();
            if (dalCusts == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "No Customers in database"
                };
            }

            var Custs = _mapper.Map<List<DisplayCustomerModel>>(dalCusts);
            return new Result
            {
                Success = true,
                DisplayAllCustomers = Custs
            };
        }


        public Result GetAllCustomersByBranch(string branch)
        {
            if (branch == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid Branch"
                };
            }

            var dalCusts = _context.Customers.Where(c => c.BranchLocation.Equals(branch)).ToList();
            if (dalCusts.Count == 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No customer found with branch {branch}"
                };
            }

            var custs = _mapper.Map<List<DisplayCustomerModel>>(dalCusts);
            return new Result
            {
                Success = true,
                DisplayAllCustomers = custs
            };
        }


        public Result GetAllAccountsOfCustomerId(int cid)
        {
            if (cid <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid Customer Id"
                };
            }

            var existCusId = _context.Customers.Find(cid);
            if (existCusId == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No customer found with customer Id {cid}"
                };
            }

            var dalAccounts = _context.Accounts.Where(a => a.CustomerId == cid).ToList();
            if (dalAccounts.Count == 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"No account found for customer with id {cid}"
                };
            }

            var accounts = _mapper.Map<List<DisplayAccountsModel>>(dalAccounts);
            return new Result
            {
                Success = true,
                DisplayAccounts = accounts
            };

        }


    }
}

