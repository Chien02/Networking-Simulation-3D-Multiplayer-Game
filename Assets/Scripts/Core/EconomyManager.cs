using System;

namespace NetworkingSimulation.Core
{
    public sealed class EconomyManager
    {
        public float CurrentBudget { get; private set; }

        public void SetBudget(float amount)
        {
            ThrowIfNegative(amount);
            CurrentBudget = amount;
        }

        public bool DeductBudget(float amount)
        {
            ThrowIfNegative(amount);

            if (amount > CurrentBudget)
            {
                return false;
            }

            CurrentBudget -= amount;
            return true;
        }

        public void RefundBudget(float amount)
        {
            ThrowIfNegative(amount);
            CurrentBudget += amount;
        }

        private static void ThrowIfNegative(float amount)
        {
            if (amount < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Budget amount cannot be negative.");
            }
        }
    }
}
