
using Ks.Core.Configuration;

namespace Ks.Core.Domain.Directory
{
    public class MeasureSettings : ISettings
    {
        public int BaseDimensionId { get; set; }
        public int BaseWeightId { get; set; }
    }
}