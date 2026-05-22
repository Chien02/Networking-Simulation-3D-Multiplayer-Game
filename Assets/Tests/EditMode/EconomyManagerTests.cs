using System;
using NetworkingSimulation.Core;
using NUnit.Framework;

namespace NetworkingSimulation.Tests
{
    public sealed class EconomyManagerTests
    {
        [Test]
        public void DeductAndRefundUpdateBudget()
        {
            EconomyManager economy = new EconomyManager();
            economy.SetBudget(100f);

            Assert.IsTrue(economy.DeductBudget(25f));
            Assert.AreEqual(75f, economy.CurrentBudget);

            economy.RefundBudget(10f);
            Assert.AreEqual(85f, economy.CurrentBudget);
        }

        [Test]
        public void DeductBudgetFailsWhenInsufficientFunds()
        {
            EconomyManager economy = new EconomyManager();
            economy.SetBudget(20f);

            Assert.IsFalse(economy.DeductBudget(30f));
            Assert.AreEqual(20f, economy.CurrentBudget);
        }

        [Test]
        public void NegativeAmountsThrow()
        {
            EconomyManager economy = new EconomyManager();

            Assert.Throws<ArgumentOutOfRangeException>(() => economy.SetBudget(-1f));
            Assert.Throws<ArgumentOutOfRangeException>(() => economy.DeductBudget(-1f));
            Assert.Throws<ArgumentOutOfRangeException>(() => economy.RefundBudget(-1f));
        }
    }
}
