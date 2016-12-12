using System;

namespace Ks.Core.Domain.Contract
{
    public class ContributionBenefitBank : BaseEntity
    {
        public int ContributionBenefitId { get; set; }
        public string CompleteName { get; set; }
        public string Dni { get; set; }
        public string RelationShip { get; set; }
        public double Ratio { get; set; }
        public decimal AmountToPay { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string CheckNumber { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdateOnUtc { get; set; }

        public virtual ContributionBenefit ContributionBenefit { get; set; }
    }
}