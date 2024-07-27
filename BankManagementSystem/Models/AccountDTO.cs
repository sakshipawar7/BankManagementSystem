using BankManagementSystem.DAL;
using System.Text.Json.Serialization;
namespace BankManagementSystem.Models
{
    public class AccountDTO
    {
        public int CustomerId { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
    }
}
