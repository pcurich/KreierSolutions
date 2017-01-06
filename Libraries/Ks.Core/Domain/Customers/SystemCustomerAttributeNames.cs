namespace Ks.Core.Domain.Customers
{
    public static partial class SystemCustomerAttributeNames
    {
        //Form fields
        public static string FirstName { get { return "FirstName"; } }
        public static string LastName { get { return "LastName"; } }
        public static string Gender { get { return "Gender"; } }

        public static string AdmCode { get { return "AdmCode"; } }
        public static string Dni { get { return "Dni"; } }
        public static string MilitarySituationId { get { return "MilitarySituationId"; } }
        public static string DateOfAdmission { get { return "DateOfAdmission"; } }

        public static string DateOfBirth { get { return "DateOfBirth"; } }
        public static string StreetAddress { get { return "StreetAddress"; } }
        public static string StreetAddress2 { get { return "StreetAddress2"; } }
       
        public static string CityId { get { return "CityId"; } }
        public static string CountryId { get { return "CountryId"; } }
        public static string StateProvinceId { get { return "StateProvinceId"; } }
        
        public static string Phone { get { return "Phone"; } }
        public static string Fax { get { return "Fax"; } }
        public static string CustomCustomerAttributes { get { return "CustomCustomerAttributes"; } }
        public static string PasswordRecoveryToken { get { return "PasswordRecoveryToken"; } }
        public static string PasswordRecoveryTokenDateGenerated { get { return "PasswordRecoveryTokenDateGenerated"; } }
        public static string AccountActivationToken { get { return "AccountActivationToken"; } }
        public static string LastVisitedPage { get { return "LastVisitedPage"; } }
        public static string AdminAreaSystemScopeConfiguration { get { return "AdminAreaSystemScopeConfiguration"; } }
         
        //depends on System
        public static string CurrencyId { get { return "CurrencyId"; } }
        public static string LanguageId { get { return "LanguageId"; } }
        public static string LanguageAutomaticallyDetected { get { return "LanguageAutomaticallyDetected"; } }
        public static string WorkingThemeName { get { return "WorkingThemeName"; } }

    }
}