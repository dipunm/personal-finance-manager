using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace BudgetManager.Tests
{
    [TestFixture]
    public class PartitionerTests
    {
        [Test]
        public void PartitionerShouldNotAllowPartsBiggerThanTransactionsToBeCreated()
        {
            const decimal amount = (decimal)100.00;
            const string description = "Money in the bank";
            var transaction = CreateTransaction(null, 50);
            var bucket = CreateBucket();
            var partitioner = new Partitioner();

            var act = new Action(() => partitioner.DefinePart(transaction, bucket, amount, description));

            act.ShouldThrow<InvalidOperationException>("because the amount supplied is larger than the transaction amount");
        }

        [Test]
        public void PartitionerShouldNotAllowNullBuckets()
        {
            const decimal amount = (decimal)100.00;
            const string description = "Money in the bank";
            var transaction = CreateTransaction(null, amount);
            const Bucket bucket = null;
            var partitioner = new Partitioner();

            var act = new Action(() => partitioner.DefinePart(transaction, bucket, amount, description));

            act.ShouldThrow<ArgumentNullException>("because the partitioner needs to assign each part to a valid bucket");
        }

        [Test]
        public void PartitionerShouldNotAllowNullTransactions()
        {
            const decimal amount = (decimal)100.00;
            const string description = "Money in the bank";
            const Transaction transaction = null;
            var bucket = CreateBucket(); ;
            var partitioner = new Partitioner();

            var act = new Action(() => partitioner.DefinePart(transaction, bucket, amount, description));

            act.ShouldThrow<ArgumentNullException>("because the partitioner needs to analyse the transaction before creating a part");
        }

        [Test]
        public void PartitionerShouldNotAllowNullDescription()
        {
            const decimal amount = (decimal)100.00;
            const string description = null;
            var transaction = CreateTransaction(null, amount);
            var bucket = CreateBucket();
            var partitioner = new Partitioner();

            var act = new Action(() => partitioner.DefinePart(transaction, bucket, amount, description));

            act.ShouldThrow<ArgumentException>("because parts are supposed to be humanised");
        }

        [Test]
        public void PartitionerShouldNotAllowPartsToBeCumalativelyBiggerThanTransactionsToBeCreated()
        {
            const decimal amount = (decimal)100.00;
            const string description = "Money in the bank";
            var transaction = CreateTransaction(null, 100);
            var partitioner = new Partitioner();
            var bucket = CreateBucket();

            partitioner.DefinePart(transaction, bucket, amount, description);
            var act = new Action(() => partitioner.DefinePart(transaction, bucket, 1, description));

            act.ShouldThrow<InvalidOperationException>("because the amount supplied is larger than the remaining undefined transaction amount");
        }

        [Test]
        public void PartitionerShouldBeAbleToFetchATransactionPartById()
        {
            var partitioner = new Partitioner();
            const decimal amount = (decimal)50.00;
            const string description = "Money in the bank";
            const string transactionId = "000-000-000";
            var transaction = CreateTransaction(transactionId, amount);
            var bucket = CreateBucket();

            Guid id = partitioner.DefinePart(transaction, bucket, amount, description);
            var part = partitioner.GetPart(id);

            var expectedPart = new TransactionPart(transactionId, bucket.Id, amount, description);
            part.ShouldBeEquivalentTo(expectedPart, options => options.Excluding(p => p.PartId));
        }

        [Test]
        public void PartitionerShouldBeAbleToFilterAndFetchTransactionsByTransactionId()
        {

            const decimal amount = (decimal)50.00;
            const string description = "Money in the bank";
            const string transactionId = "000-000-000";
            const string transaction2Id = "000-000-001";
            var transaction = CreateTransaction(transactionId, amount * 2);
            var transaction2 = CreateTransaction(transaction2Id, amount);
            var partitioner = new Partitioner();
            var bucket = CreateBucket();

            var partId1 = partitioner.DefinePart(transaction, bucket, amount, description);
            var partId2 = partitioner.DefinePart(transaction, bucket, amount, description);
            var partId3 = partitioner.DefinePart(transaction2, bucket, amount, description);
            var parts = partitioner.GetTransactionParts(transactionId);

            parts.Should().BeEquivalentTo(new[]
            {
                partitioner.GetPart(partId1),
                partitioner.GetPart(partId2)
            });
        }

        [Test]
        public void PartitionerShouldBeAbleToCalculateExcessUnpartitionedAmountFromTransaction()
        {
            const decimal amount = (decimal)50.00;
            const decimal excess = 10;
            const string description = "Money in the bank";
            var transaction = CreateTransaction(amount: amount + excess);
            var partitioner = new Partitioner();
            var bucket = CreateBucket();
            partitioner.DefinePart(transaction, bucket, amount, description);

            var unpartitionedAmount = partitioner.CalculateUnpartitionedAmount(transaction);

            unpartitionedAmount.Should().Be(excess);
        }

        [Test]
        public void PartitionerShouldCalculateUnpartitionedAmountAsTransactionTotalIfTransactionNeverPartitioned()
        {
            const decimal amount = (decimal)50.00;
            var transaction = CreateTransaction(amount: amount);
            var partitioner = new Partitioner();

            var unpartitionedAmount = partitioner.CalculateUnpartitionedAmount(transaction);

            unpartitionedAmount.Should().Be(amount, "because there is nothing to subtract from transaction amount");
        }

        [Test]
        public void DefinePartShouldAssociatePartWithBucket()
        {
            var bucket = CreateBucket();
            var transaction = CreateTransaction();
            const decimal amount = 100;
            const string description = "money in the bank";
            var partitioner = new Partitioner();

            var partId = partitioner.DefinePart(transaction, bucket, amount, description);
            var part = partitioner.GetPart(partId);

            part.BucketId.Should().Be(bucket.Id);
        }

        [Test]
        public void ShouldBeAbleToFilterPartsByBucket()
        {
            var transaction = CreateTransaction();
            var bucket1 = CreateBucket();
            var bucket2 = CreateBucket();
            const decimal amount = 3;
            const string description = "money in the bank";
            var partitioner = new Partitioner();

            var partId1 = partitioner.DefinePart(transaction, bucket1, amount, description);
            var partId2 = partitioner.DefinePart(transaction, bucket1, amount, description);
            var partId3 = partitioner.DefinePart(transaction, bucket1, amount, description);
            var partId4 = partitioner.DefinePart(transaction, bucket2, amount, description);
            var partId5 = partitioner.DefinePart(transaction, bucket2, amount, description);

            IEnumerable<TransactionPart> parts = partitioner.GetPartsInBucket(bucket1.Id);

            parts.Should().BeEquivalentTo(new[]
            {
                partitioner.GetPart(partId1),
                partitioner.GetPart(partId2),
                partitioner.GetPart(partId3)
            });
        }

        [Test]
        public void ShouldBeAbleToRemoveParts()
        {
            var transaction = CreateTransaction();
            var bucket1 = CreateBucket();
            const decimal amount = 3;
            const string description = "money in the bank";
            var partitioner = new Partitioner();

            var partId = partitioner.DefinePart(transaction, bucket1, amount, description);
            partitioner.RemovePart(partId);

            partitioner.GetPart(partId).Should().BeNull();
        }

        [Test]
        public void ShouldBeAbleToUpdateAPartsAmountEntity()
        {
            var transaction = CreateTransaction();
            var bucket1 = CreateBucket();
            const decimal amount = 3;
            const string description = "money in the bank";
            var partitioner = new Partitioner();
            var partId = partitioner.DefinePart(transaction, bucket1, amount, description);

            partitioner.ChangeAmount(partId, transaction, transaction.Amount);
            var part = partitioner.GetPart(partId);

            part.Amount.Should().Be(transaction.Amount, "because I changed the amount to be the same as the transactions amount");
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ShouldNotAcceptAmountLessOrEqualZero(decimal amount)
        {
            var transaction = CreateTransaction();
            var bucket1 = CreateBucket();
            const string description = "money in the bank";
            var partitioner = new Partitioner();

            var act = (Action)(() => partitioner.DefinePart(transaction, bucket1, amount, description));

            act.ShouldThrow<ArgumentException>("because a partition of zero or less is bad");
        }

        [Test]
        public void ShouldThrowExceptionIfUnknownPartChangeAttempted()
        {
            var transaction = CreateTransaction();
            const decimal amount = 10;
            var partitioner = new Partitioner();

            var act = (Action)(() => partitioner.ChangeAmount(Guid.NewGuid(), transaction, amount));

            act.ShouldThrow<ArgumentException>("because a partition not found");
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ShouldNotAcceptAmountLessOrEqualZeroWhenChangingValue(decimal amount)
        {
            const decimal validAmount = 20;
            const string description = "money in the bank";
            var transaction = CreateTransaction(amount: validAmount);
            var bucket1 = CreateBucket();
            var partitioner = new Partitioner();

            var partId = partitioner.DefinePart(transaction, bucket1, validAmount, description);
            var act = (Action)(() => partitioner.ChangeAmount(partId, transaction, amount));

            act.ShouldThrow<ArgumentException>("because a partition of zero or less is bad");
        }

        [Test]
        public void ShouldNotAcceptAmountIfCanOverPartitionTransaction()
        {
            const string description = "money in the bank";
            const decimal validAmount = 20;
            var transaction = CreateTransaction(amount: validAmount * 2);
            var bucket1 = CreateBucket();
            var partitioner = new Partitioner();

            var partId1 = partitioner.DefinePart(transaction, bucket1, validAmount, description);
            var partId2 = partitioner.DefinePart(transaction, bucket1, validAmount, description);
            var act = (Action)(() => partitioner.ChangeAmount(partId1, transaction, validAmount + 10));

            act.ShouldThrow<InvalidOperationException>("because new amount would cause sum of partitions to exceed transaction total");
        }

        [Test]
        public void PartsCanBeMovedBetweenBuckets()
        {
            var bucket = CreateBucket("Bucket1");
            var bucket2 = CreateBucket("Bucket2");
            var transaction = CreateTransaction();
            const decimal amount = 50;
            const string description = "money in the bank";
            var partitioner = new Partitioner();
            var partId = partitioner.DefinePart(transaction, bucket, amount, description);
            
            partitioner.MovePart(partId, bucket2);

            partitioner.GetPart(partId).BucketId.Should().Be(bucket2.Id);
        }

        [Test]
        public void PartCanChangeDescription()
        {
            var bucket = CreateBucket();
            var transaction = CreateTransaction();
            const decimal amount = 50;
            const string description = "money in the bank";
            var partitioner = new Partitioner();
            var partId = partitioner.DefinePart(transaction, bucket, amount, description);

            const string description2 = "money in the shop";
            partitioner.ChangeDescription(partId, description2);

            partitioner.GetPart(partId).Description.Should().Be(description2);
        }

        private Transaction CreateTransaction(string transactionId = null, decimal? amount = null)
        {
            var transaction = new Transaction(
                transactionId: transactionId ?? "100-000-000",
                bank: "Halifax",
                sourceAccountId: "00-03-40:00645170",
                amount: amount.HasValue ? amount.Value : 100,
                description: "DD: COMPANYNAME 30490983975",
                date: new Date(01, 01, 2015)
                );
            return transaction;
        }

        private Bucket CreateBucket(string name = "Bucket1")
        {
            return new Bucket(
                id: Guid.NewGuid(),
                name: name
            );
        }
    }
}
