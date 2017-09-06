using System;

namespace Ks.Core.Domain.Reports
{
    public class Report : BaseEntity
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string PathBase { get; set; }
        public int StateId { get; set; }
        public string Period { get; set; }
        public string Source { get; set; }
        public Guid ParentKey { get; set; }
        public DateTime DateUtc { get; set; }

        public ReportState ReportState
        {
            get { return (ReportState)StateId; }
            set { StateId = (int)value; }
        }

    }
}