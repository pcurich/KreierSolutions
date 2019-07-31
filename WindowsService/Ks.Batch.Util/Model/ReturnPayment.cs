using System; 

namespace Ks.Batch.Util.Model
{
    public class ReturnPayment
    {
        public int Id { get; set; }
        public int ReturnPaymentTypeId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentNumber { get; set; }
        public decimal AmountToPay { get; set; }
        public int StateId { get; set; }

        public ReturnPaymentState ReturnPaymentState
        {
            get { return (ReturnPaymentState)StateId; }
            set { StateId = (int)value; }
        }

        public ReturnPaymentType ReturnPaymentType
        {
            get { return (ReturnPaymentType)ReturnPaymentTypeId; }
            set { ReturnPaymentTypeId = (int)value; }
        }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        #region Bank

        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string CheckNumber { get; set; }

        #endregion 
    }
}
