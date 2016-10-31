using Ks.Core.Domain.Reports;

namespace Ks.Data.Mapping.Reports
{
    public class ReportMap : KsEntityTypeConfiguration<Report>
    {
        public ReportMap()
        {
            ToTable("Reports");
            HasKey(r => r.Id);

            Property(r => r.Value).HasColumnType("xml");
            Ignore(u => u.ReportState);

        }
    }
}