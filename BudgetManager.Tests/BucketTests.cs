using System;
using FluentAssertions;
using NUnit.Framework;

namespace BudgetManager.Tests
{
    [TestFixture]
    public class BucketTests
    {

        [Test]
        public void BucketShouldNotAcceptEmptyGuidAsId()
        {
            var act = new Action(() => new Bucket(Guid.Empty, "Bucket1"));
            act.ShouldThrow<ArgumentException>("because an empty guid is not unique.");
        }

        [Test]
        public void BucketShouldUseIdFromConstructorForIdProperty()
        {
            var id = Guid.NewGuid();
            var bucket = new Bucket(id, "Bucket1");

            bucket.Id.Should().Be(id);
        }

        [Test]
        public void BucketShouldNotAcceptNullName()
        {
            var act = new Action(() => new Bucket(Guid.NewGuid(), null));
            act.ShouldThrow<ArgumentException>("because an empty name is not readable.");
        }

        [Test]
        public void BucketShouldNotAcceptEmptyName()
        {
            var act = new Action(() => new Bucket(Guid.NewGuid(), String.Empty));
            act.ShouldThrow<ArgumentException>("because an empty name is not readable.");
        }

        [Test]
        public void BucketShouldNotAcceptWhitespaceAsName()
        {
            var act = new Action(() => new Bucket(Guid.NewGuid(), "   "));
            act.ShouldThrow<ArgumentException>("because an empty name is not readable.");
        }
    }
}