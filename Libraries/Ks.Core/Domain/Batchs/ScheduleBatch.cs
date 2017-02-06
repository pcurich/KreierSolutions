using System;

namespace Ks.Core.Domain.Batchs
{
    public class ScheduleBatch : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the system.
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the path base.
        /// </summary>
        public string PathBase { get; set; }

        /// <summary>
        /// Gets or sets the folder read.
        /// </summary>
        public string FolderRead { get; set; }
        /// <summary>
        /// Gets or sets the folder log.
        /// </summary>
        public string FolderLog { get; set; }
        /// <summary>
        /// Gets or sets the folder move to done.
        /// </summary>
        public string FolderMoveToDone { get; set; }
        /// <summary>
        /// Gets or sets the folder move to error.
        /// </summary>
        public string FolderMoveToError { get; set; }
        /// <summary>
        /// Gets or sets the frecuency identifier.
        /// </summary>
        public int FrecuencyId { get; set; }
        /// <summary>
        /// Gets or sets the period year.
        /// </summary>
        public int PeriodYear { get; set; }
        /// <summary>
        /// Gets or sets the period month.
        /// </summary>
        public int PeriodMonth { get; set; }
        /// <summary>
        /// Gets or sets the start execution on UTC.
        /// </summary>
        public DateTime? StartExecutionOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the next execution on UTC.
        /// </summary>
        public DateTime? NextExecutionOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the last execution on UTC.
        /// </summary>
        public DateTime? LastExecutionOnUtc { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScheduleBatch"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the schedule batch frecuency.
        /// </summary>
        public ScheduleBatchFrecuency ScheduleBatchFrecuency
        {
            get { return (ScheduleBatchFrecuency)FrecuencyId; }
            set { FrecuencyId = (int)value; }
        }

        public bool UpdateData { get; set; }
    }
}