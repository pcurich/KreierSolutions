using Ks.Admin.Models.Common;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    public partial class CustomerAddressModel : BaseKsModel
    {
        public int CustomerId { get; set; }

        public AddressModel Address { get; set; }
    }
}