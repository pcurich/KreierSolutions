using System;

namespace Ks.Core.Domain.Contract
{
    public class Check : BaseEntity
    {
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public int EntityTypeId { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string CheckNumber { get; set; }
        public decimal Amount { get; set; }
        public int CheckStateId { get; set; }

        public CheckSatate CheckSatate
        {
            get { return (CheckSatate) CheckStateId; }
            set { CheckStateId = (int) value; }
        }

        public string Reason { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}