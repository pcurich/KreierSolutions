using Ks.Core.Configuration;
namespace Ks.Core.Domain.Contract
{
    public class BenefitValueSetting : ISettings
    {
        public decimal AmountBaseOfBenefit { get; set; }
    }
}