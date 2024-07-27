using BankManagementSystem.DAL;
using BankManagementSystem.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BankManagementSystem.Services;
using BankManagementSystem.Repo;
using System.ComponentModel.DataAnnotations;

namespace BankManagementSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AccountServiceDelegate _delegate;
        private readonly BankDbContext _context;
        private readonly ITransactionRepo _transactionRepo;

        public TransactionController(AccountServiceDelegate delegates, BankDbContext context, ITransactionRepo transactionRepo)
        {
            _delegate = delegates;
            _context = context;
            _transactionRepo = transactionRepo;
        }

        [HttpPost("RevertTransaction")]
        public IActionResult RevertTransaction([Required] int transactionId)
        {
            var result = _transactionRepo.RevertTransaction(transactionId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.SuccessMsg);
        }



    }
}
//var accType =  _context.Accounts
//              .Where(a=>a.AccountId == accId && !a.IsDeleted)
//              .Select(a=>a.AccountType)
//              .FirstOrDefault();