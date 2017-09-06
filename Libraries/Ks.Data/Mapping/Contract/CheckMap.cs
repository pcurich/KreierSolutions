using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class CheckMap: KsEntityTypeConfiguration<Check>
    {
        public CheckMap()
        {
            ToTable("Check");
            HasKey(sp => sp.Id);
            Property(sp => sp.EntityId).IsRequired();
            Property(sp => sp.EntityName).IsRequired();

            Property(sp => sp.AccountNumber).IsRequired();
            Property(sp => sp.BankName).IsRequired();
            Property(sp => sp.CheckNumber).IsRequired();
            Property(sp => sp.Amount).IsRequired();
            Property(sp => sp.CheckStateId).IsRequired();

            Property(sp => sp.Reason).IsRequired();
            Property(sp => sp.Amount).HasPrecision(12, 2);

            Ignore(sp => sp.CheckSatate);
        } 
    }
}