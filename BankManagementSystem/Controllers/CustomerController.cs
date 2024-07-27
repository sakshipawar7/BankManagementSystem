using BankManagementSystem.Models;
using BankManagementSystem.DAL;
using BankManagementSystem.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BankManagementSystem.Helpers;
using BankManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System.ComponentModel.DataAnnotations;

namespace BankManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepo _customerRepo;

        public CustomerController(ICustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }


        [HttpPost("CreateCustomer")]
        public IActionResult CreateCustomer([FromBody] [Required] CustomerAccountHybridModel customerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _customerRepo.CreateCustomer(customerModel);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.SuccessMsg);            
        }

        
        [HttpPatch("UpdateCustomer")]
        public IActionResult UpdateCustomer([Required] int id, [FromBody] JsonPatchDocument<UpdateCustomerModel> patchDoc)
        {
            var result = _customerRepo.UpdateCustomer(id, patchDoc);

            if (!result.Success)
            {
                if (result.ValidationErrors != null && result.ValidationErrors.Any())
                {
                    return BadRequest(new
                    {
                        Message = result.ErrorMessage,
                        Errors = result.ValidationErrors
                    });
                }

                return NotFound(result.ErrorMessage);
            }

            return Ok(new
            {
                result.SuccessMsg,
                result.Data                
            });
        }



        [HttpGet("GetCustomerDetailsById")]
        public IActionResult GetCustomerDetailsById([Required] int cid)
        {
            var result = _customerRepo.GetCustomerDetailsById(cid);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.DisplayCustomer);
        }



        [HttpGet("DisplayAllCustomers")]
        public IActionResult DisplayAllCustomers()
        {
            var result = _customerRepo.DisplayAllCustomers();
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.DisplayAllCustomers);
        }



        [HttpGet("GetAllCustomersByBranch")]
        public IActionResult GetAllCustomersByBranch([Required] string branch)
        {
            var result = _customerRepo.GetAllCustomersByBranch(branch);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.DisplayAllCustomers);
        }



        [HttpGet("GetAllAccountsOfCustomerId")]
        public IActionResult GetAllAccountsOfCustomerId([Required] int cid)
        {
            var result = _customerRepo.GetAllAccountsOfCustomerId(cid);
            if(!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.DisplayAccounts);
        }

        [HttpDelete("DeleteCustomer")]
        public IActionResult DeleteCustomer([Required] int customerId)
        {
            var result = _customerRepo.DeleteCustomer(customerId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.SuccessMsg);
        }



        //[HttpPatch("Update Customer")]
        //public IActionResult UpdateCustomer(int id, [FromBody] JsonPatchDocument<UpdateCustomerModel> patchDoc)
        //{
        //    if (patchDoc == null)
        //    {
        //        return BadRequest("Patch document is null.");
        //    }

        //    // Call the repository method
        //    var result = _customerRepo.UpdateCustomer(id, patchDoc);

        //    // Check the result for success or failure
        //    if (!result.Success)
        //    {
        //        // If the result indicates failure, return the error message
        //        return NotFound(result.ErrorMessage);
        //    }

        //    // Return the updated customer data or a success message
        //    return Ok(result.Data ?? "Update successful.");
        //}

    }
}
