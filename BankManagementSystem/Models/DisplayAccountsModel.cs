using BankManagementSystem.DAL;

namespace BankManagementSystem.Models
{
    public class DisplayAccountsModel
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime AccountConvertedDate { get; set; }
        public int RemainingWithdrawlAmountPerDay { get; set; }
        public byte RemainingNoOfWithdrawlsPerDay { get; set; }
        public DateTime DateOfDeletion { get; set; }

        public bool IsDeleted { get; set; } = false;

        

    }
}
