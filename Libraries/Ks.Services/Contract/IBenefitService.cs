using System;
using System.Collections.Generic;
using Ks.Core;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Reports;

namespace Ks.Services.Contract
{
    public interface IBenefitService
    {
        #region Benefit
        void DeleteBenefit(Benefit benefit);
        void InsertBenefit(Benefit benefit);
        void UpdateBenefit(Benefit benefit);
        Benefit GetBenefitById(int benefitId);
        List<Benefit> GetActiveBenefits();
        IPagedList<Benefit> GetAllBenefits(int pageIndex = 0, int pageSize = Int32.MaxValue);
        #endregion

        #region ContributionBenefit

        void DeleteContributionBenefit(ContributionBenefit contributionBenefit);
        void InsertContributionBenefit(ContributionBenefit contributionBenefit);
        void UpdateContributionBenefit(ContributionBenefit contributionBenefit);

        ContributionBenefit GetContributionBenefitbyId(int contributionBenefitId);
        IPagedList<ContributionBenefit> GetAllContributionBenefitByCustomer(int customerId = 0,  int pageIndex = 0, int pageSize = Int32.MaxValue);


        #endregion

        #region ContributionBenefitBank

        void DeleteContributionBenefitBank(ContributionBenefitBank contributionBenefitBank);
        void InsertContributionBenefitBank(ContributionBenefitBank contributionBenefitBank);
        void UpdateContributionBenefitBank(ContributionBenefitBank contributionBenefitBank);
        ContributionBenefitBank GetContributionBenefitBankById(int contributionBenefitBankId);

        IPagedList<ContributionBenefitBank> GetAllContributionBenefitBank(int contributionBenefitId, int pageIndex = 0, int pageSize = Int32.MaxValue);

        #endregion

        #region Report

        IList<ReportContributionBenefit> GetReportContributionBenefit(int contributionBenefitId, int pageIndex = 0,
            int pageSize = Int32.MaxValue);


        #endregion
    }
}