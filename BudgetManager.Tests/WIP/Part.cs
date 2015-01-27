using System;

namespace BudgetManager.Tests
{
    public class Part
    {
        private Guid _partId;
        private decimal _amount;
        private string _description;
        private string _transactionId;
        private Guid _bucketId;

        public Part(string transactionId, Guid bucketId, decimal amount, string description)
        {
            _partId = Guid.NewGuid();
            TransactionId = transactionId;
            BucketId = bucketId;
            Amount = amount;
            Description = description;
        }

        public Guid PartId
        {
            get { return _partId; }
        }

        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("amount cannot be less than or equal to zero", "value");
                _amount = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("description cannot be null or whitespace", "value");
                _description = value;
            }
        }

        public string TransactionId
        {
            get { return _transactionId; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("transactionId cannot be null or whitespace", "value");
                _transactionId = value;
            }
        }

        public Guid BucketId
        {
            get { return _bucketId; }
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Guid cannot be equal to emptyGuid", "value");

                _bucketId = value;
            }
        }
    }
}