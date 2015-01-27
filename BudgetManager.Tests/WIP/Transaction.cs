namespace BudgetManager.Tests
{
    public class Transaction
    {

        public Transaction(string transactionId, string bank, string sourceAccountId, decimal amount, string description, Date date)
        {
            TransactionId = transactionId;
            Amount = amount;
        }

        public decimal Amount { get; private set; }
        public string TransactionId { get; private set; }
    }
}