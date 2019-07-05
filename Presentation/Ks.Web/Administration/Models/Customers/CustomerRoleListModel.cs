using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ks.Admin.Models.Customers
{
    public class CustomerRoleListModel
    {
        public int CustomerRoleId { get; set; }
        public string CustomerDni { get; set; }
        public string CustomerAdminCode { get; set; }
        public bool IsNew { get; set; }

    }
}