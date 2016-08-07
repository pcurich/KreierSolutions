using System.Collections.Generic;
using Ks.Web.Framework.Mvc;
using Ks.Web.Models.Common;

namespace Ks.Web.Models.Customer
{
    public partial class CustomerAddressListModel : BaseKsModel
    {
        public CustomerAddressListModel()
        {
            Addresses = new List<AddressModel>();
        }

        public IList<AddressModel> Addresses { get; set; }
    }
}