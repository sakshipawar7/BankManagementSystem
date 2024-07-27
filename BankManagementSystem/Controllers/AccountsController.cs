//using BankManagementSystem.Repo;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;

//namespace BankManagementSystem.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AccountsController : ControllerBase
//    {
//        private readonly IAccountsRepo _accountsRepo;
//        public AccountsController(IAccountsRepo accountsRepo)
//        {
//            _accountsRepo = accountsRepo;
//        }

//        [HttpGet("Get All Accounts")]
//        public IActionResult GetAllAccounts([Required] string accountTypeStr)
//        {
//            if (TryGetAccountService(accountTypeStr, out var accountService, out var accountType) is IActionResult badRequestResult)
//            {
//                return badRequestResult;
//            }

//            var result = accountService.GetAllAccounts(accountType);
//            if (!result.Success)
//            {
//                return BadRequest(result.ErrorMessage);
//            }
//            return Ok(result.DisplayAccounts);
//        }
//    }
//}
