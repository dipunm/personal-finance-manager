using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BudgetManager.Tests
{
    public class Partitioner
    {
        private readonly List<Part> _parts;

        public Partitioner()
        {
            _parts = new List<Part>();
        }

        public TransactionPart GetPart(Guid transactionPartId)
        {
            return _parts.Where(p => p.PartId == transactionPartId)
                .Select(p => new TransactionPart(p)).SingleOrDefault();
        }

        public IEnumerable<TransactionPart> GetTransactionParts(string transactionId)
        {
            return _parts.Where(p => p.TransactionId == transactionId).Select(p => new TransactionPart(p));
        }

        public decimal CalculateUnpartitionedAmount(Transaction transaction)
        {
            var partitionedAmount = _parts
                .Where(p => p.TransactionId == transaction.TransactionId)
                .Sum(p => p.Amount);
            return transaction.Amount - partitionedAmount;
        }

        public Guid DefinePart(Transaction transaction, Bucket bucket, decimal amount, string description)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if(bucket == null)
                throw new ArgumentNullException("bucket");
            
            EnforceAmountValidation(transaction, amount);

            var part = new Part(transaction.TransactionId, bucket.Id, amount, description);
            _parts.Add(part);
            return part.PartId;
        }

        private void EnforceAmountValidation(Transaction transaction, decimal amount)
        {
            if (transaction.Amount < amount)
                throw new InvalidOperationException("Cannot create part larger than transaction.");

            var cumulativeAmount = _parts
                .Where(p => p.TransactionId == transaction.TransactionId)
                .Sum(p => p.Amount);
            if (cumulativeAmount + amount > transaction.Amount)
                throw new InvalidOperationException(
                    "Amount supplied would exceed total transaction amount when combined with other parts from this transaction.");
        }

        public IEnumerable<TransactionPart> GetPartsInBucket(Guid bucketId)
        {
            return _parts.Where(p => p.BucketId == bucketId)
                .Select(p => new TransactionPart(p));
        }

        public void RemovePart(Guid partId)
        {
            _parts.RemoveAll(p => p.PartId == partId);
        }

        public void ChangeAmount(Guid partId, Transaction transaction, decimal amount)
        {
            var part = _parts.SingleOrDefault(p => p.PartId == partId);
            if(part == null)
                throw new ArgumentException("Part with id supplied not found to modify.", "partId");
            EnforceAmountValidation(transaction, amount - part.Amount);
            part.Amount = amount;
        }

        public void MovePart(Guid partId, Bucket bucket2)
        {
            var part = _parts.Single(p => p.PartId == partId);
            part.BucketId = bucket2.Id;
        }

        public void ChangeDescription(Guid partId, string description)
        {
            var part = _parts.Single(p => p.PartId == partId);
            part.Description = description;
        }
    }
}