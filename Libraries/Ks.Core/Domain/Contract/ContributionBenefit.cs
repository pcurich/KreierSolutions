using System;
using System.Collections.Generic;

namespace Ks.Core.Domain.Contract
{
    public class ContributionBenefit : BaseEntity
    {
        private ICollection<ContributionBenefitBank> _contributionBenefitBanks;
        public int BenefitId { get; set; }
        public int ContributionId { get; set; }
        public int NumberOfLiquidation { get; set; }
        public decimal AmountBaseOfBenefit { get; set; }
        public int YearInActivity { get; set; }
        public double TabValue { get; set; }
        public double Discount { get; set; }
        public decimal SubTotalToPay { get; set; }

        #region Contribution

        public decimal TotalContributionCaja { get; set; }
        public decimal TotalContributionCopere { get; set; }
        public decimal TotalContributionPersonalPayment { get; set; }
        public decimal ReserveFund { get; set; }

        #endregion

        #region Loan
        public int TotalLoan { get; set; }
        public decimal TotalLoanToPay { get; set; }
        #endregion

        public string CustomField1 { get; set; }
        public string CustomValue1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomValue2 { get; set; }
        public decimal TotalToPay { get; set; }
        public int TotalReationShip { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public virtual Benefit Benefit { get; set; }
        public virtual Contribution Contribution { get; set; }
        public virtual ICollection<ContributionBenefitBank> ContributionBenefitBanks
        {
            get { return _contributionBenefitBanks ?? (_contributionBenefitBanks = new List<ContributionBenefitBank>()); }
            protected set { _contributionBenefitBanks = value; }
        }

    }
}