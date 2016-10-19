using System.Web.Mvc.Html;
using Ks.Core.Configuration;

namespace Ks.Core.Domain.Contract
{
    public class BankSettings : ISettings
    {
        public bool IsActive1 { get; set; }
        public string NameBank1 { get; set; }
        public string AccountNumber1 { get; set; }
        public bool IsActive2 { get; set; }
        public string NameBank2 { get; set; }
        public string AccountNumber2 { get; set; }
        public bool IsActive3 { get; set; }
        public string NameBank3 { get; set; }
        public string AccountNumber3 { get; set; }
        public bool IsActive4 { get; set; }
        public string NameBank4 { get; set; }
        public string AccountNumber4 { get; set; }
        public bool IsActive5 { get; set; }
        public string NameBank5 { get; set; }
        public string AccountNumber5 { get; set; }
    }
}