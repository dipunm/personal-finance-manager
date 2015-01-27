using System;
using System.Security.Cryptography;

namespace BudgetManager.Tests
{
    public class TransactionPart
    {
        private readonly Part _part;

        public TransactionPart(Part part)
        {
            _part = part;
        }

        public TransactionPart(string transactionId, Guid bankId, decimal amount, string description) 
            : this(new Part(transactionId, bankId, amount, description))
        {
            
        }

        public Guid PartId { get { return _part.PartId; } }
        public decimal Amount { get { return _part.Amount; } }
        public string Description { get { return _part.Description; } }
        public string TransactionId { get { return _part.TransactionId; } }
        public Guid BucketId { get { return _part.BucketId; } }

        public override bool Equals(object obj)
        {
            var part2 = obj as TransactionPart;
            if (part2 == null)
                return false;

            return part2.PartId == PartId;
        }

    }
}