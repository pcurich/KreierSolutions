using System;
using System.Collections.Generic;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;

namespace Ks.Core.Domain.Reports
{
    public class ReportCustomer
    {
        public ReportCustomer()
        {
            CustomerAttributes = new List<CustomerAttribute>();
            Loans = new List<Loan>();
        }

        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string AdmCode { get; set; }
        public string Dni { get; set; }
        public int MilitarySituationId { get; set; }
        public string MilitarySituationName { get; set; }
        public int DeclaratoryLetter { get; set; }
        public DateTime? DateOfAdmission { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int StateProvinceId { get; set; }
        public string StateProvinceName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; }
        public string CustomerRoleNames { get; set; }

        public List<CustomerAttribute> CustomerAttributes { get; set; }
        public Contribution Contribution { get; set; }
        public List<Loan> Loans { get; set; }
    }
}