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
        /// Gets or sets the path read.
        /// </summary>
        public string PathRead { get; set; }
        /// <summary>
        /// Gets or sets the path log.
        /// </summary>
        public string PathLog { get; set; }
        /// <summary>
        /// Gets or sets the path move to done.
        /// </summary>
        public string PathMoveToDone { get; set; }
        /// <summary>
        /// Gets or sets the path move to error.
        /// </summary>
        public string PathMoveToError { get; set; }
        /// <summary>
        /// Gets or sets the frecuency identifier.
        /// </summary>
        public int FrecuencyId { get; set; }
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
    }
}