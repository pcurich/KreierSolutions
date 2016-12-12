using System;

namespace Ks.Core.Domain.Contract
{
    public class Benefit : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BenefitTypeId { get; set; }
        public double Discount { get; set; }
        public bool CancelLoans { get; set; }
        public bool LetterDeclaratory { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public BenefitType BenefitType
        {
            get { return (BenefitType)BenefitTypeId; }
            set { BenefitTypeId = (int)value; }
        }
    }
}