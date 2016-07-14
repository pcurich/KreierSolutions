using Ks.Core.Domain.Media;

namespace Ks.Data.Mapping.Media
{
    public partial class DownloadMap :KsEntityTypeConfiguration<Download>
    {
        public DownloadMap()
        {
            ToTable("Download");
            HasKey(p => p.Id);
            Property(p => p.DownloadBinary).IsMaxLength();
        }
    }
}