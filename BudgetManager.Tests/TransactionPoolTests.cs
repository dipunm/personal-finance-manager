using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace BudgetManager.Tests
{
    /// <summary>
    /// We have Transactions
    /// Transactions are read only
    /// Transactions can have their own ID's
    /// They will be filtered by a number of fields:
    /// - Source (halifax)
    /// - Source (Account 1)
    /// - Amount (less than 100)
    /// - Name (Description from bank)
    /// - Date (of transaction)
    /// - Description (may include any excess data from source)
    /// 
    /// We have buckets.
    /// Buckets are user defined
    /// They have names (user friendly identifiers)
    /// They can have budgets
    /// They hold TransactionParts
    /// 
    /// Budgets:
    /// Must have an amount
    /// May have a reset occurrance
    /// 
    /// </summary>
    [TestFixture]
    public class TransactionPoolTests
    {/*
        public void Monkey()
        {
            var transactions = new[]
            {
                new Transaction(
                    "000-000-000",
                    "bank1",
                    "popmsadm",
                    100,
                    "madomoasd",
                    new Date(01, 01, 2015))
            };
            var pool = new TransactionPool();
            pool.AddTransactions(transactions);
            var transcations = pool.GetTransactions();

            transactions.ShouldAllBeEquivalentTo(transactions);
        }
        */
    }
}
