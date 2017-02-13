using System;

namespace Ks.Batch.Util
{
    public class ScheduleBatch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
        public string PathBase { get; set; }
        public string FolderRead { get; set; }
        public string FolderLog { get; set; }
        public string FolderMoveToDone { get; set; }
        public string FolderMoveToError { get; set; }
        public int FrecuencyId { get; set; }
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public DateTime? StartExecutionOnUtc { get; set; }
        public DateTime? NextExecutionOnUtc { get; set; }
        public DateTime? LastExecutionOnUtc { get; set; }
        public bool Enabled { get; set; }
        public bool UpdateData { get; set; }

        public override string ToString()
        {
            return "{SystemName=" + SystemName +
                ",PeriodYear=" + PeriodYear +
                ",PeriodMonth=" + PeriodMonth +
                ",StartExecution=" + StartExecutionOnUtc +
                ",NextExecution=" + NextExecutionOnUtc +
                ",LastExecution=" + LastExecutionOnUtc +
                ",Enabled=" + Enabled +
                ",UpdateData=" + UpdateData +"}";
        }

        public bool Equals(ScheduleBatch obj)
        {
            return Id == obj.Id && Name.Trim() == obj.Name.Trim() && SystemName.Trim() == obj.SystemName.Trim() &&
                   PathBase.Trim() == obj.PathBase.Trim() &&
                   FolderRead.Trim() == obj.FolderRead.Trim() && FolderLog.Trim() == obj.FolderLog.Trim() &&
                   FolderMoveToDone.Trim() == obj.FolderMoveToDone.Trim() &&
                   FolderMoveToError.Trim() == obj.FolderMoveToError.Trim() &&
                   FrecuencyId == obj.FrecuencyId && PeriodYear == obj.PeriodYear && PeriodMonth == obj.PeriodMonth &&
                   StartExecutionOnUtc == obj.StartExecutionOnUtc && NextExecutionOnUtc == obj.NextExecutionOnUtc &&
                   LastExecutionOnUtc == obj.LastExecutionOnUtc && Enabled == obj.Enabled;
        }
    }
}