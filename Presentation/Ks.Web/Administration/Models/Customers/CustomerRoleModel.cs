using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Customers;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    [Validator(typeof(CustomerRoleValidator))]
    public partial class CustomerRoleModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Customers.CustomerRoles.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerRoles.Fields.FreeShipping")]
        [AllowHtml]
        public bool FreeShipping { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerRoles.Fields.TaxExempt")]
        public bool TaxExempt { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerRoles.Fields.Active")]
        public bool Active { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerRoles.Fields.IsSystemRole")]
        public bool IsSystemRole { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerRoles.Fields.SystemName")]
        public string SystemName { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct")]
        public int PurchasedWithProductId { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct")]
        public string PurchasedWithProductName { get; set; }


        #region Nested classes

        public partial class AssociateProductToCustomerRoleModel : BaseKsModel
        {
            public AssociateProductToCustomerRoleModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [KsResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [KsResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [KsResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [KsResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [KsResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [KsResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }


            public int AssociatedToProductId { get; set; }
        }
        #endregion
    }
}