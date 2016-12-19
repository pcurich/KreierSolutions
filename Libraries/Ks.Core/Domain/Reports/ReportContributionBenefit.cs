using System;
using Ks.Core.Domain.Contract;

namespace Ks.Core.Domain.Reports
{
    public class ReportContributionBenefit
    {
        public string BenefitName { get; set; }
        public string BenefitType { get; set; }
        public int NumberOfLiquidation { get; set; }
        public decimal AmountBaseOfBenefit { get; set; }
        public int YearInActivity { get; set; }
        public double TabValue { get; set; }
        public double Discount { get; set; }
        public decimal SubTotalToPay { get; set; }
        public decimal TotalContributionCaja { get; set; }
        public decimal TotalContributionCopere { get; set; }
        public decimal TotalContributionPersonalPayment { get; set; }
        public int TotalLoan { get; set; }
        public decimal TotalLoanToPay { get; set; }
        public decimal ReserveFund { get; set; }
        public int TotalReationShip { get; set; }
        public decimal TotalToPay { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string Checks { get; set; }
    }
}