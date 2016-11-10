using System;

namespace Ks.Batch.Util.Model
{
    public class Reports
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string PathBase { get; set; }
        public int StateId { get; set; }
        public string Period { get; set; }
        public string Source { get; set; }
        public Guid ParentKey { get; set; }
        public DateTime DateUtc { get; set; }
    }
}