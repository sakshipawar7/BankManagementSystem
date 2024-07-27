using BankManagementSystem.DAL;
using BankManagementSystem.Helpers;
using BankManagementSystem.Services;
using System.Globalization;
namespace BankManagementSystem.Repo
{
    public interface ITransactionRepo
    {
        public Result DepositAndAddTransaction(int accId, decimal amount);
        public Result WithdrawAndAddTransaction(int accId, decimal amount);
        public Result TransferAndAddTransaction(int senderId, int receiverId, int amount);
        //public Result DeleteTransactionByTransactionId(int tid);
        //public Result UndoTransactionDeletionOfTransactionID(int tid);
        public Result RevertTransaction(int tid);
    }
    public class TransactionRepo : ITransactionRepo
    {
        private readonly BankDbContext _context;

        public TransactionRepo(BankDbContext context)
        {
            _context = context;
        }

        public Result DepositAndAddTransaction(int accId, decimal amount)
        {
            if(accId <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Account Id cannot be {accId}"
                };
            }

            if (amount <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Amount cannot be negative or 0"
                };
            }

            var dalAccount = _context.Accounts.Find(accId);
            if (dalAccount == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Account Id {accId} not found"
                };
            }

            
            dalAccount.Balance += amount;

            var transaction = new Transaction
            {
                SenderAccountId = accId,
                TransactionType = TransactionType.Deposit,
                Amount = amount,
                TCreationDate = DateTime.Now,
                ReceiverAccountId = accId,
                Description = $"Account Id {accId} credited with amount {amount}"
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return new Result
            {
                Success = true,
                SuccessMsg = $"Successfully deposited {amount} into account {accId}. New balance is {dalAccount.Balance}."
            };

        }

        public Result WithdrawAndAddTransaction(int accId, decimal amount)
        {
            if (accId <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Account Id cannot be {accId}"
                };
            }

            if (amount <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Amount cannot be negative or 0"
                };
            }

            var dalAccount = _context.Accounts.FirstOrDefault(a=>a.AccountId == accId && !a.IsDeleted);
            if (dalAccount == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Account Id {accId} not found"
                };
            }

            if (amount > dalAccount.Balance)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Withdrawal Rejected. \r\nInsufficient balance. Current Balance is {dalAccount.Balance} rupees."
                };
            }


            if (dalAccount.AccountType == AccountType.Savings)
            {             

                if (dalAccount.Balance - amount < AccountConstants.SavingsAccountMinBalance)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Withdrawal Rejected. \r\nWithdrawal would result in balance dropping below the minimum required balance of {AccountConstants.SavingsAccountMinBalance:C}."
                    };
                }

                //DateTime utcNow = DateTime.UtcNow;
                //TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                //DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, istTimeZone);
                //if (istNow - dalAccount.LastResetDate > TimeSpan.FromDays(1))
                //{
                //    dalAccount.RemainingWithdrawlAmountPerDay = AccountConstants.MaxWithdrawlAmountPerDaySavings;
                //    dalAccount.RemainingNoOfWithdrawlsPerDay = AccountConstants.MaxNoOfWithdrawlsPerDaySavings;
                //    dalAccount.LastResetDate = DateTime.UtcNow;
                //}
                CheckLastReset.Check(dalAccount);

                if (amount > dalAccount.RemainingWithdrawlAmountPerDay)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Withdrawal Rejected. \r\nExceeds daily withdrawal limit. You can withdraw only {dalAccount.RemainingWithdrawlAmountPerDay} rupees."
                    };
                }

                if (dalAccount.RemainingNoOfWithdrawlsPerDay <= 0)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = "Withdrawal Rejected. \r\nDaily withdrawal limit reached. You can withdraw money after tonight at 12 AM."
                    };
                }
                                              
            }
            
            
            
            else if(dalAccount.AccountType == AccountType.Current)
            {

                if (dalAccount.Balance - amount < AccountConstants.CurrentAccountMinBalance)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Withdrawal Rejected. \r\nWithdrawal would result in balance dropping below the minimum required balance of {AccountConstants.CurrentAccountMinBalance:C}."
                    };
                }

                //if (DateTime.UtcNow - dalAccount.LastResetDate > TimeSpan.FromDays(1))
                //{
                //    dalAccount.RemainingWithdrawlAmountPerDay = AccountConstants.MaxWithdrawlAmountPerDayCurrent;
                //    dalAccount.RemainingNoOfWithdrawlsPerDay = AccountConstants.MaxNoOfWithdrawlsPerDayCurrent;
                //    dalAccount.LastResetDate = DateTime.UtcNow;
                //}
                CheckLastReset.Check(dalAccount);

                if (amount > dalAccount.RemainingWithdrawlAmountPerDay)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Withdrawal Rejected. \r\nExceeds daily withdrawal limit. You can withdraw only {dalAccount.RemainingWithdrawlAmountPerDay} rupees."
                    };
                }

                if (dalAccount.RemainingNoOfWithdrawlsPerDay <= 0)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = "Withdrawal Rejected. \r\nDaily withdrawal limit reached. You can withdraw money after tonight at 12 AM."
                    };
                }                        

            }

            dalAccount.Balance -= amount;
            dalAccount.RemainingWithdrawlAmountPerDay -= (int)amount;
            dalAccount.RemainingNoOfWithdrawlsPerDay -= 1;

            _context.Accounts.Update(dalAccount);
            _context.SaveChanges();

            var transaction = new Transaction
            {
                SenderAccountId = accId,
                TransactionType = TransactionType.Withdraw,
                Amount = amount,
                TCreationDate = DateTime.Now,
                ReceiverAccountId = accId,
                Description = $"Withdrawal Successful. \r\nAccount Id {accId} debited with amount {ConvertDollarSymbolToRupeeSymbol(amount)}."
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return new Result
            {
                Success = true,
                SuccessMsg = $"Withdrawal successful.\r\nRemaining balance : {dalAccount.Balance} " +
                $"\r\nRemaining number of withdrawals for the day : {dalAccount.RemainingNoOfWithdrawlsPerDay} " +
                $"\r\nRemaining Withdrawal amount for the day : {dalAccount.RemainingWithdrawlAmountPerDay}"
            };
        }


        public Result TransferAndAddTransaction(int senderId, int receiverId ,int amount)
        {
            if(senderId == receiverId)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Sender and Receiver Id cannot be same"
                };
            }

            if (senderId <= 0 || receiverId <= 0 )
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Invalid sender or receiver account ID."
                };
            }

            if (amount <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Amount cannot be negative or 0"
                };
            }

            var senderAccount = _context.Accounts.FirstOrDefault(a => a.AccountId == senderId && !a.IsDeleted);
            var receiverAccount = _context.Accounts.FirstOrDefault(a => a.AccountId == receiverId && !a.IsDeleted);

            if (senderAccount == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Sender account with ID {senderId} not found."
                };
            }

            if (receiverAccount == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Receiver account with ID {receiverId} not found."
                };
            }

            if (senderAccount.Balance < amount)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Transfer Rejected. \r\nInsufficient balance. Current Balance is {ConvertDollarSymbolToRupeeSymbol(senderAccount.Balance)}."
                };
            }

            
            if (senderAccount.AccountType == AccountType.Savings)
            {

                if (senderAccount.Balance - amount < AccountConstants.SavingsAccountMinBalance)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Transfer Rejected. \r\nWithdrawal would result in balance dropping below the minimum required balance of {ConvertDollarSymbolToRupeeSymbol(AccountConstants.SavingsAccountMinBalance)}."
                    };
                }

                if (DateTime.Now - senderAccount.LastResetDate > TimeSpan.FromDays(1))
                {
                    senderAccount.RemainingWithdrawlAmountPerDay = AccountConstants.MaxWithdrawlAmountPerDaySavings;
                    senderAccount.RemainingNoOfWithdrawlsPerDay = AccountConstants.MaxNoOfWithdrawlsPerDaySavings;
                    senderAccount.LastResetDate = DateTime.Now;
                }

                if (amount > senderAccount.RemainingWithdrawlAmountPerDay)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Transfer Rejected. \r\nExceeds daily withdrawal limit. You can withdraw only {senderAccount.RemainingWithdrawlAmountPerDay} rupees."
                    };
                }

                if (senderAccount.RemainingNoOfWithdrawlsPerDay <= 0)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = "Transfer Rejected. \r\nDaily withdrawal limit reached. You can withdraw money after tonight at 12 AM."
                    };
                }

            }



            else if (senderAccount.AccountType == AccountType.Current)
            {

                if (senderAccount.Balance - amount < AccountConstants.CurrentAccountMinBalance)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Transfer Rejected. \r\nWithdrawal would result in balance dropping below the minimum required balance of {ConvertDollarSymbolToRupeeSymbol(AccountConstants.CurrentAccountMinBalance)}."
                    };
                }

                if (DateTime.Now - senderAccount.LastResetDate > TimeSpan.FromDays(1))
                {
                    senderAccount.RemainingWithdrawlAmountPerDay = AccountConstants.MaxWithdrawlAmountPerDayCurrent;
                    senderAccount.RemainingNoOfWithdrawlsPerDay = AccountConstants.MaxNoOfWithdrawlsPerDayCurrent;
                    senderAccount.LastResetDate = DateTime.Now;
                }

                if (amount > senderAccount.RemainingWithdrawlAmountPerDay)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Transfer Rejected. \r\nExceeds daily withdrawal limit. You can withdraw only {senderAccount.RemainingWithdrawlAmountPerDay} rupees."
                    };
                }

                if (senderAccount.RemainingNoOfWithdrawlsPerDay <= 0)
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = $"Transfer Rejected. \r\nDaily withdrawal limit reached. You can withdraw money after {DateTime.Now - senderAccount.LastResetDate}"
                    };
                }

            }

            senderAccount.RemainingNoOfWithdrawlsPerDay -= 1;
            senderAccount.RemainingWithdrawlAmountPerDay -= amount;

            senderAccount.Balance -= amount;
            receiverAccount.Balance += amount;



            var transaction = new Transaction
            {
                SenderAccountId = senderId,
                ReceiverAccountId = receiverId,
                TransactionType = TransactionType.Transfer,
                Amount = amount,
                TCreationDate = DateTime.Now,
                Description = $"Transferred {ConvertDollarSymbolToRupeeSymbol(amount)} to account {receiverId}"
            };

            _context.Transactions.Add(transaction);

            try
            {
                _context.Accounts.Update(senderAccount);
                _context.Accounts.Update(receiverAccount);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "An error occurred while processing the transaction."
                };
            }

            return new Result
            {
                Success = true,
                SuccessMsg = $"Transfer Successful.\r\nTransfer of {ConvertDollarSymbolToRupeeSymbol(amount)} from account {senderId} to account {receiverId} was successful.\r\n" +
                     $"Sender's remaining balance: {ConvertDollarSymbolToRupeeSymbol(senderAccount.Balance)}\r\n" +
                     $"Receiver's new balance: {ConvertDollarSymbolToRupeeSymbol(receiverAccount.Balance)}\r\n" +
                     $"Remaining number of withdrawals for the sender: {senderAccount.RemainingNoOfWithdrawlsPerDay}\r\n" +
                     $"Remaining withdrawal amount for the sender: {senderAccount.RemainingWithdrawlAmountPerDay:C}"
            };
                       
        }


        public Result RevertTransaction(int tid)
        {
            if (tid <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Transaction Id cannot be 0 or negative"
                };
            }

            var transaction = _context.Transactions.FirstOrDefault(t => t.TransactionId == tid && !t.IsDeleted);
            if (transaction == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Transaction doesn't exist."
                };
            }

            var result = TransferAndAddTransaction(transaction.ReceiverAccountId, transaction.SenderAccountId, (int)transaction.Amount);
            if (!result.Success)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Transaction revert failed"
                };
            }
            return new Result
            {
                Success = true,
                SuccessMsg = $"Transaction reverted.\r\n {transaction.Amount} rupees reverted and sent from Account Id : {transaction.ReceiverAccountId} to Account Id : {transaction.SenderAccountId}"
            };
        }

        //public Result DeleteTransactionByTransactionId(int tid)
        //{
        //    if(tid <= 0)
        //    {
        //        return new Result
        //        {
        //            Success = false,
        //            ErrorMessage = "Transaction Id cannot be 0 or negative"
        //        };
        //    }

        //    var transaction = _context.Transactions.Find(tid);
        //    if (transaction == null)
        //    {
        //        return new Result
        //        {
        //            Success = false,
        //            ErrorMessage = $"Transaction doesn't exist."
        //        };
        //    }

        //    transaction.IsDeleted = true;
        //    _context.Transactions.Update(transaction);
        //    _context.SaveChanges();

        //    return new Result
        //    {
        //        Success = true,
        //        SuccessMsg = "Transaction deleted successfully"
        //    };
        //}



        //public Result UndoTransactionDeletionOfTransactionID(int tid)
        //{
        //    if(tid <= 0)
        //    {
        //        return new Result
        //        {
        //            Success = false,
        //            ErrorMessage = "Transaction Id cannot be 0 or negative"
        //        };
        //    }

        //    var transaction = _context.Transactions.FirstOrDefault(t=> t.TransactionId == tid && t.IsDeleted);
        //    if (transaction == null)
        //    {
        //        return new Result
        //        {
        //            Success = false,
        //            ErrorMessage = $"Transaction doesn't exist."
        //        };
        //    }

        //    transaction.IsDeleted = false;
        //    _context.Transactions.Update(transaction);
        //    _context.SaveChanges();

        //    return new Result
        //    {
        //        Success = true,
        //        SuccessMsg = "Transaction restored successfully"
        //    };
        //}



        // ALL HELPER FUNCTIONS HERE
        public static string ConvertDollarSymbolToRupeeSymbol(decimal amount)
        {
            string formattedAmount = amount.ToString("C", new CultureInfo("en-IN"));
            return formattedAmount;
        }

    }
}
