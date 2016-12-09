using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class BenefitMap: KsEntityTypeConfiguration<Benefit>
    {
        public BenefitMap()
        {
            ToTable("Benefit");
            HasKey(sp => sp.Id);
            Property(sp => sp.Name).IsRequired();

            Ignore(sp => sp.BenefitType);
        }
    }
    
}