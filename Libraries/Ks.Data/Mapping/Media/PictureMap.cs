using Ks.Core.Domain.Media;

namespace Ks.Data.Mapping.Media
{
    public partial class PictureMap : KsEntityTypeConfiguration<Picture>
    {
        public PictureMap()
        {
            ToTable("Picture");
            HasKey(p => p.Id);
            Property(p => p.PictureBinary).IsMaxLength();
            Property(p => p.MimeType).IsRequired().HasMaxLength(40);
            Property(p => p.SeoFilename).HasMaxLength(300);
        }
    }
}