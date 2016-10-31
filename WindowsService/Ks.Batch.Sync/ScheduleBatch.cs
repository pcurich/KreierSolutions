using System;

namespace Ks.Batch.Sync
{
    public class ScheduleBatch
    {
        public int Id { get;set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
        public string PathRead { get; set; }
        public string PathLog { get; set; }
        public string PathMoveToDone { get; set; }
        public string PathMoveToError { get; set; }
        public int FrecuencyId { get; set; }
        public DateTime? StartExecutionOnUtc { get; set; }
        public DateTime? NextExecutionOnUtc { get; set; }
        public DateTime? LastExecutionOnUtc { get; set; }
        public bool Enabled { get; set; }
    }
}