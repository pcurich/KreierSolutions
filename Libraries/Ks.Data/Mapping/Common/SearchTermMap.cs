using Ks.Core.Domain.Common;

namespace Ks.Data.Mapping.Common
{
    public partial class SearchTermMap : KsEntityTypeConfiguration<SearchTerm>
    {
        public SearchTermMap()
        {
            ToTable("SearchTerm");
            HasKey(st => st.Id);
        }
    }
}
