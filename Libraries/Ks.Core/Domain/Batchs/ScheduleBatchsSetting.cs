using Ks.Core.Configuration;

namespace Ks.Core.Domain.Batchs
{
    public class ScheduleBatchsSetting : ISettings
    {
        public string ServiceName1 { get; set; }
        public int DayOfProcess1 { get; set; }
        public string ServiceName2 { get; set; }
        public int DayOfProcess2 { get; set; }
    }
}