using Ks.Batch.Util.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Ks.Batch.Test
{
    public class BaseTest
    {
        public void Contribution(List<InfoContribution> result, int size = 1, int contributionPaymentId = 0, decimal amountPayed = 0, int stateId = 0)
        {
            Assert.IsTrue(result.Count(x=>x.ContributionPaymentId == contributionPaymentId) == size);
            List<InfoContribution> filter = result.Where(x => x.ContributionPaymentId == contributionPaymentId).ToList();
            foreach (InfoContribution c in filter)
            {
                Assert.AreEqual(c.ContributionPaymentId, contributionPaymentId);
                Assert.AreEqual(c.AmountPayed, amountPayed);
                Assert.AreEqual(c.StateId, stateId);
            }
        }

        public void Loan(List<InfoLoan> result, int loanId = 0, int size = 0, decimal monthlyQuota = 0M, decimal monthlyPayed = 0M, int stateId = 0)
        {
            Assert.IsTrue(result.Count(x => x.LoanId == loanId) == size);
            Assert.AreEqual(result.FirstOrDefault(x => x.LoanId == loanId).Quota, 1);
            Assert.AreEqual(result.FirstOrDefault(x => x.LoanId == loanId).MonthlyQuota, monthlyQuota);
            Assert.AreEqual(result.FirstOrDefault(x => x.LoanId == loanId).MonthlyPayed, monthlyPayed);
            Assert.AreEqual(result.FirstOrDefault(x => x.LoanId == loanId).StateId, stateId);
        }

        public void LoanPayment(List<InfoLoan> result, int loanId = 0, int loanPaymentId = 0, int size = 0, decimal monthlyQuota = 0M, decimal monthlyPayed = 0M, int stateId = 0)
        {
            Assert.IsTrue(result.Count(x => x.LoanId == loanId) == size);
            Assert.AreEqual(result.FirstOrDefault(x => x.LoanPaymentId == loanPaymentId).Quota, 1);
            Assert.AreEqual(result.FirstOrDefault(x => x.LoanPaymentId == loanPaymentId).MonthlyQuota, monthlyQuota);
            Assert.AreEqual(result.FirstOrDefault(x => x.LoanPaymentId == loanPaymentId).MonthlyPayed, monthlyPayed);
            Assert.AreEqual(result.FirstOrDefault(x => x.LoanPaymentId == loanPaymentId).StateId, stateId);
        }

        public void NextPayment(List<InfoLoan> result, List<InfoLoan> dataBase, int loanId = 0, int loanPaymentId = 0, int size = 0, decimal monthlyQuota = 0, int stateId = (int)LoanState.Pendiente)
        {
            Assert.IsTrue(result.Count(x => x.LoanId == loanId) == size);
            Assert.AreEqual(result.Where(x => x.LoanId == loanId).Sum(X => X.MonthlyQuota), monthlyQuota);
            if (result.Count == 1)
            {
                Assert.AreEqual(result.FirstOrDefault(x => x.LoanId == loanId).StateId, stateId);
            }
            else
            {
                foreach (var r in result)
                {
                    Assert.AreEqual(r.StateId, stateId);
                }
            }

        }

        public void ReturnPayment(List<Info> result, int customerId = 0, decimal totalPayed = 0)
        {
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(result[0].CustomerId, customerId);
            Assert.AreEqual(result[0].TotalPayed, totalPayed);
        }
    }
}
