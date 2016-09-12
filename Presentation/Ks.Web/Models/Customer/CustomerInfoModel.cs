using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;
using Ks.Web.Validators.Customer;

namespace Ks.Web.Models.Customer
{
    [Validator(typeof(CustomerInfoValidator))]
    public partial class CustomerInfoModel : BaseKsModel
    {
        public CustomerInfoModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
            this.AvailableCountries = new List<SelectListItem>();
            this.AvailableStates = new List<SelectListItem>();
            this.AvailableCities = new List<SelectListItem>();
            this.AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            this.CustomerAttributes = new List<CustomerAttributeModel>();
        }

        [KsResourceDisplayName("Account.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }
        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }
        [KsResourceDisplayName("Account.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [KsResourceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        [KsResourceDisplayName("Account.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }
        [KsResourceDisplayName("Account.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }

        [KsResourceDisplayName("Account.Fields.Cpi")]
        [AllowHtml]
        public string Cpi { get; set; }
        [KsResourceDisplayName("Account.Fields.Dni")]
        [AllowHtml]
        public string Dni { get; set; }


        public bool DateOfBirthEnabled { get; set; }
        [KsResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthDay { get; set; }
        [KsResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthMonth { get; set; }
        [KsResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthYear { get; set; }
        public bool DateOfBirthRequired { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }

        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        [KsResourceDisplayName("Account.Fields.Company")]
        [AllowHtml]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        [KsResourceDisplayName("Account.Fields.StreetAddress")]
        [AllowHtml]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        [KsResourceDisplayName("Account.Fields.StreetAddress2")]
        [AllowHtml]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        [KsResourceDisplayName("Account.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        public bool CountryEnabled { get; set; }
        public bool CountryRequired { get; set; }
        [KsResourceDisplayName("Account.Fields.Country")]
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public bool StateProvinceRequired { get; set; }
        [KsResourceDisplayName("Account.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        [KsResourceDisplayName("Account.Fields.City")]
        public int CityId { get; set; }
        public IList<SelectListItem> AvailableCities { get; set; }

        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        [KsResourceDisplayName("Account.Fields.Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        [KsResourceDisplayName("Account.Fields.Fax")]
        [AllowHtml]
        public string Fax { get; set; }

        public bool NewsletterEnabled { get; set; }
        [KsResourceDisplayName("Account.Fields.Newsletter")]
        public bool Newsletter { get; set; }

        //preferences
        public bool SignatureEnabled { get; set; }
        [KsResourceDisplayName("Account.Fields.Signature")]
        [AllowHtml]
        public string Signature { get; set; }

        //time zone
        [KsResourceDisplayName("Account.Fields.TimeZone")]
        public string TimeZoneId { get; set; }
        public bool AllowCustomersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [KsResourceDisplayName("Account.Fields.VatNumber")]
        [AllowHtml]
        public string VatNumber { get; set; }
        public string VatNumberStatusNote { get; set; }
        public bool DisplayVatNumber { get; set; }

        //external authentication
        [KsResourceDisplayName("Account.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }
        public int NumberOfExternalAuthenticationProviders { get; set; }

        public IList<CustomerAttributeModel> CustomerAttributes { get; set; }

        #region Nested classes

        public partial class AssociatedExternalAuthModel : BaseKsEntityModel
        {
            public string Email { get; set; }

            public string ExternalIdentifier { get; set; }

            public string AuthMethodName { get; set; }
        }

        #endregion
    }
}