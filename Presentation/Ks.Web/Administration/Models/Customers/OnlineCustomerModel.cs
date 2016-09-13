using System;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    public partial class OnlineCustomerModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.CustomerInfo")]
        public string CustomerInfo { get; set; }

        [KsResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [KsResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.Location")]
        public string Location { get; set; }

        [KsResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        [KsResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }
    }
}