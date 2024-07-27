using BankManagementSystem.DAL;
using BankManagementSystem.Models;
using System.Collections;
namespace BankManagementSystem.Helpers
{
    public class Result
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
        public IEnumerable<string> ValidationErrors { get; set; }

        public DisplayCustomerModel DisplayCustomer { get; set; }
        public IEnumerable<DisplayCustomerModel> DisplayAllCustomers { get; set; }
        public IEnumerable<DisplayAccountsModel> DisplayAccounts { get; set; }

        public string SuccessMsg    { get; set; }
        
    }
}
