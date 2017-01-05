using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Web.Framework;

namespace Ks.Admin.Models.Customers
{
    public class CustomerListModel
    {
        public CustomerListModel()
        {
            MonthOfBirthValues =new List<SelectListItem>();
            DayOfBirthValues = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        [AllowHtml]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }

        [KsResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        public int[] SearchCustomerRoleIds { get; set; }

        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }

        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchUsername")]
        [AllowHtml]
        public string SearchUsername { get; set; }
        public bool UsernamesEnabled { get; set; }

        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchFirstName")]
        [AllowHtml]
        public string SearchFirstName { get; set; }
        [KsResourceDisplayName  ("Admin.Customers.Customers.List.SearchLastName")]
        [AllowHtml]
        public string SearchLastName { get; set; }

        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchAdmCode")]
        [AllowHtml]
        public string SearchAdmCode { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchDni")]
        [AllowHtml]
        public string SearchDni { get; set; }


        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchDayOfBirth { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchMonthOfBirth { get; set; }
        public bool DateOfBirthEnabled { get; set; }

        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchPhone")]
        [AllowHtml]
        public string SearchPhone { get; set; }
        public bool PhoneEnabled { get; set; }

        [KsResourceDisplayName("Admin.Customers.Customers.List.SearchZipCode")]
        [AllowHtml]
        public string SearchZipPostalCode { get; set; }
        public bool ZipPostalCodeEnabled { get; set; }

        public List<SelectListItem> MonthOfBirthValues { get; set; }
        public List<SelectListItem> DayOfBirthValues { get; set; }
    }
}