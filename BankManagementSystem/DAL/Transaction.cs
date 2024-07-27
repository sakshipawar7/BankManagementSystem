namespace BankManagementSystem.DAL
{
    public enum TransactionType
    {
        Deposit,
        Withdraw,
        Transfer
    }
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int SenderAccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TCreationDate { get; set; }
        public int ReceiverAccountId { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual Account SenderAccount { get; set; } // The account making the transaction
        public virtual Account ReceiverAccount { get; set; } // The account receiving the transaction

    }
}
