using System;
using System.Collections.Generic;
using Ks.Core;
using Ks.Core.Domain.Contract;

namespace Ks.Services.Contract
{
    public interface ICheckService
    {
        IPagedList<Check> SearchCheck(DateTime? from = null, DateTime? to = null, string entityName = null,
            string bankName = null, string checkNumber = null, int pageIndex = 0, int pageSize = Int32.MaxValue);

        Check GetCheck(int checkId);
        void Replace(Loan loan, Check check);
        void Replace(ReturnPayment retunPayment, Check check);
        void Replace(ContributionBenefitBank contributionBenefitBank, Check check);
        IList<Check> GetChecks(DateTime? from = null, DateTime? to = null, string entityName = null);
    }
}