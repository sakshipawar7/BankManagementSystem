using BankManagementSystem.DAL;

namespace BankManagementSystem.Models
{
    public class TransactionModel
    {
        //public int TransactionId { get; set; }
        public int SenderAccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TCreationDate { get; set; }
        public int ReceiverAccountId { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
