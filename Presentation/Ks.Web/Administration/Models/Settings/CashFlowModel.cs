using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    public class CashFlowModel
    {
        public int Id { get; set; }
        public decimal Since { get; set; }
        public decimal To { get; set; }
        public decimal Amount { get; set; }
    }
}