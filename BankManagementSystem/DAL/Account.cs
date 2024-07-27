using System.ComponentModel.DataAnnotations;
namespace BankManagementSystem.DAL
{
    public enum AccountType
    {
        All,
        Savings,
        Current,
    }
    public class Account
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }

        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime AccountConvertedDate { get; set; }
        public int RemainingWithdrawlAmountPerDay { get; set; }
        public byte RemainingNoOfWithdrawlsPerDay { get; set; }
        public DateTime DateOfDeletion { get; set; }
        public DateTime LastResetDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public Customer Customer { get; set; }
    }
}
