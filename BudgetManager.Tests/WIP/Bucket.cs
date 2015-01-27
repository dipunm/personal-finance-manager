using System;

namespace BudgetManager.Tests
{
    public class Bucket
    {
        public Bucket(Guid id, string name)
        {
            if(String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name cannot be null or empty.");
            if(id == Guid.Empty)
                throw new ArgumentException("Guid cannot be equal to emptyGuid", "id");

            Id = id;
        }

        public Guid Id { get; private set; }
    }
}