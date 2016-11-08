using System;

namespace Ks.Core.Domain.Reports
{
    public class Report : BaseEntity
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Gets or sets the name of report.
        /// </summary>
        public string Name { get; set; }
        public string Value { get; set; }
        public string PathBase { get; set; }
        public int StateId { get; set; }
        public string Period { get; set; }
        /// <summary>
        /// Gets or sets the source of this report.
        /// </summary>
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