using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class ReturnPaymentMap : KsEntityTypeConfiguration<ReturnPayment>
    {
        public ReturnPaymentMap()
        {
            ToTable("ReturnPayment");
            HasKey(sp => sp.Id);
            Property(sp => sp.AmountToPay).HasPrecision(12, 2);

            Ignore(sp => sp.ReturnPaymentType);
            Ignore(sp => sp.ReturnPaymentState);
        }
    }
}