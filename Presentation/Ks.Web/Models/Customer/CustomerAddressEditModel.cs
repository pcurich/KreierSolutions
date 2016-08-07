using Ks.Web.Framework.Mvc;
using Ks.Web.Models.Common;

namespace Ks.Web.Models.Customer
{
    public partial class CustomerAddressEditModel : BaseKsModel
    {
        public CustomerAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        public AddressModel Address { get; set; }
    }
}