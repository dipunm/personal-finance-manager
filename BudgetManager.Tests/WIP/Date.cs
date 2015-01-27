using System;

namespace BudgetManager.Tests
{
    public class Date
    {
        private DateTime _date;

        public Date(int day, int month, int year)
        {
            _date = new DateTime(year, month, day);
        }
    }
}