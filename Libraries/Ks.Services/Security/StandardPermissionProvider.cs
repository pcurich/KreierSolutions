using System.Collections.Generic;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Security;

namespace Ks.Services.Security
{
    /// <summary>
    /// Standard permission provider
    /// </summary>
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };

        //Sistemas
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "Admin area. Manage System Log", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new PermissionRecord { Name = "Admin area. Manage Message Queue", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance = new PermissionRecord { Name = "Admin area. Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleTasks = new PermissionRecord { Name = "Admin area. Manage Schedule Tasks", SystemName = "ManageScheduleTasks", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleBatchs = new PermissionRecord { Name = "Admin area. Manage Schedule Batchs", SystemName = "ManageScheduleBatchs", Category = "Configuration" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "Admin area. Manage Countries", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Admin area. Manage Languages", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Admin area. Manage Settings", SystemName = "ManageSettings", Category = "Configuration" };



        public static readonly PermissionRecord ManageCustomers = new PermissionRecord { Name = "Admin area. Manage Customers", SystemName = "ManageCustomers", Category = "Customers" };
        public static readonly PermissionRecord ManageCustomerRoles = new PermissionRecord { Name = "Admin area. Manage Customer Role", SystemName = "ManageCustomerRole", Category = "Customers" };

        public static readonly PermissionRecord ManageContributionBenefit = new PermissionRecord { Name = "Admin area. Manage Contribution Benefit", SystemName = "ManageContributionBenefit", Category = "Customers" };
        public static readonly PermissionRecord ManageContributions = new PermissionRecord { Name = "Admin area. Manage Contributions", SystemName = "ManageContributions", Category = "Customers" };
        public static readonly PermissionRecord ManageLoans = new PermissionRecord { Name = "Admin area. Manage Loans", SystemName = "ManageLoans", Category = "Customers" };
        public static readonly PermissionRecord ManageTabs = new PermissionRecord { Name = "Admin area. Manage Tabs", SystemName = "ManageTabs", Category = "Content Management" };
        public static readonly PermissionRecord ManageBenefits = new PermissionRecord { Name = "Admin area. Manage Benefits", SystemName = "ManageBenefits", Category = "Customers" };
        public static readonly PermissionRecord ManageChecks = new PermissionRecord { Name = "Admin area. Manage Checks", SystemName = "ManageChecks", Category = "Customers" };
        public static readonly PermissionRecord ManageReturns = new PermissionRecord { Name = "Admin area. Manage Returns", SystemName = "ManageReturns", Category = "Customers" };
        public static readonly PermissionRecord ManageReports = new PermissionRecord { Name = "Admin area. Manage Reports", SystemName = "ManageReports", Category = "Customers" };

        public static readonly PermissionRecord ApprovalLoan = new PermissionRecord { Name = "Admin area. Approval Loan", SystemName = "ApprovalLoan", Category = "Customers" };
        public static readonly PermissionRecord ApprovalBenefit = new PermissionRecord { Name = "Admin area. Approval Benefit", SystemName = "ApprovalBenefit", Category = "Customers" };


        public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "Admin area. Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "Admin area. Manage Email Accounts", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageKsSystem = new PermissionRecord { Name = "Admin area. Manage Systems", SystemName = "ManageSystem", Category = "Configuration" };


        //public store permissions

        public static readonly PermissionRecord PublicStoreAllowNavigation = new PermissionRecord { Name = "Public store. Allow navigation", SystemName = "PublicStoreAllowNavigation", Category = "PublicStore" };
        


        //No va
        public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "Admin area. Manage ACL", SystemName = "ManageACL", Category = "Configuration" };

        //public static readonly PermissionRecord AccessClosedStore = new PermissionRecord { Name = "Public store. Access a closed store", SystemName = "AccessClosedStore", Category = "PublicStore" };
        //public static readonly PermissionRecord DisplayPrices = new PermissionRecord { Name = "Public store. Display Prices", SystemName = "DisplayPrices", Category = "PublicStore" };
        //public static readonly PermissionRecord EnableShoppingCart = new PermissionRecord { Name = "Public store. Enable shopping cart", SystemName = "EnableShoppingCart", Category = "PublicStore" };
        //public static readonly PermissionRecord EnableWishlist = new PermissionRecord { Name = "Public store. Enable wishlist", SystemName = "EnableWishlist", Category = "PublicStore" };
        //public static readonly PermissionRecord ManagePaymentMethods = new PermissionRecord { Name = "Admin area. Manage Payment Methods", SystemName = "ManagePaymentMethods", Category = "Configuration" };
        //public static readonly PermissionRecord ManageExternalAuthenticationMethods = new PermissionRecord { Name = "Admin area. Manage External Authentication Methods", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };
        //public static readonly PermissionRecord ManageTaxSettings = new PermissionRecord { Name = "Admin area. Manage Tax Settings", SystemName = "ManageTaxSettings", Category = "Configuration" };
        //public static readonly PermissionRecord ManageShippingSettings = new PermissionRecord { Name = "Admin area. Manage Shipping Settings", SystemName = "ManageShippingSettings", Category = "Configuration" };
        //public static readonly PermissionRecord ManageCurrencies = new PermissionRecord { Name = "Admin area. Manage Currencies", SystemName = "ManageCurrencies", Category = "Configuration" };
        //public static readonly PermissionRecord ManageMeasures = new PermissionRecord { Name = "Admin area. Manage Measures", SystemName = "ManageMeasures", Category = "Configuration" };
        //public static readonly PermissionRecord ManageAttributes = new PermissionRecord { Name = "Admin area. Manage Attributes", SystemName = "ManageAttributes", Category = "Catalog" };
        //public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "Admin area. Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };
        //public static readonly PermissionRecord AllowCustomerImpersonation = new PermissionRecord { Name = "Admin area. Allow Customer Impersonation", SystemName = "AllowCustomerImpersonation", Category = "Customers" };
        //public static readonly PermissionRecord ManageProducts = new PermissionRecord { Name = "Admin area. Manage Products", SystemName = "ManageProducts", Category = "Catalog" };
        //public static readonly PermissionRecord ManageCategories = new PermissionRecord { Name = "Admin area. Manage Categories", SystemName = "ManageCategories", Category = "Catalog" };
        //public static readonly PermissionRecord ManageManufacturers = new PermissionRecord { Name = "Admin area. Manage Manufacturers", SystemName = "ManageManufacturers", Category = "Catalog" };
        //public static readonly PermissionRecord ManageProductReviews = new PermissionRecord { Name = "Admin area. Manage Product Reviews", SystemName = "ManageProductReviews", Category = "Catalog" };
        //public static readonly PermissionRecord ManageProductTags = new PermissionRecord { Name = "Admin area. Manage Product Tags", SystemName = "ManageProductTags", Category = "Catalog" };
        //public static readonly PermissionRecord ManageForums = new PermissionRecord { Name = "Admin area. Manage Forums", SystemName = "ManageForums", Category = "Content Management" };
        //public static readonly PermissionRecord ManageVendors = new PermissionRecord { Name = "Admin area. Manage Vendors", SystemName = "ManageVendors", Category = "Customers" };
        //public static readonly PermissionRecord ManageCurrentCarts = new PermissionRecord { Name = "Admin area. Manage Current Carts", SystemName = "ManageCurrentCarts", Category = "Orders" };
        //public static readonly PermissionRecord ManageOrders = new PermissionRecord { Name = "Admin area. Manage Orders", SystemName = "ManageOrders", Category = "Orders" };
        //public static readonly PermissionRecord ManageRecurringPayments = new PermissionRecord { Name = "Admin area. Manage Recurring Payments", SystemName = "ManageRecurringPayments", Category = "Orders" };
        //public static readonly PermissionRecord ManageGiftCards = new PermissionRecord { Name = "Admin area. Manage Gift Cards", SystemName = "ManageGiftCards", Category = "Orders" };
        //public static readonly PermissionRecord ManageReturnRequests = new PermissionRecord { Name = "Admin area. Manage Return Requests", SystemName = "ManageReturnRequests", Category = "Orders" };
        //public static readonly PermissionRecord OrderCountryReport = new PermissionRecord { Name = "Admin area. Access order country report", SystemName = "OrderCountryReport", Category = "Orders" };
        //public static readonly PermissionRecord ManageAffiliates = new PermissionRecord { Name = "Admin area. Manage Affiliates", SystemName = "ManageAffiliates", Category = "Promo" };
        //public static readonly PermissionRecord ManageCampaigns = new PermissionRecord { Name = "Admin area. Manage Campaigns", SystemName = "ManageCampaigns", Category = "Promo" };
        //public static readonly PermissionRecord ManageDiscounts = new PermissionRecord { Name = "Admin area. Manage Discounts", SystemName = "ManageDiscounts", Category = "Promo" };
        //public static readonly PermissionRecord ManageNewsletterSubscribers = new PermissionRecord { Name = "Admin area. Manage Newsletter Subscribers", SystemName = "ManageNewsletterSubscribers", Category = "Promo" };
        //public static readonly PermissionRecord ManagePolls = new PermissionRecord { Name = "Admin area. Manage Polls", SystemName = "ManagePolls", Category = "Content Management" };
        //public static readonly PermissionRecord ManageNews = new PermissionRecord { Name = "Admin area. Manage News", SystemName = "ManageNews", Category = "Content Management" };
        //public static readonly PermissionRecord ManageBlog = new PermissionRecord { Name = "Admin area. Manage Blog", SystemName = "ManageBlog", Category = "Content Management" };
        //public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "Admin area. Manage Widgets", SystemName = "ManageWidgets", Category = "Content Management" };
        //public static readonly PermissionRecord ManageTopics = new PermissionRecord { Name = "Admin area. Manage Topics", SystemName = "ManageTopics", Category = "Content Management" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "Admin area. Manage Message Templates", SystemName = "ManageMessageTemplates", Category = "Content Management" };
        public static readonly PermissionRecord HtmlEditorManagePictures = new PermissionRecord { Name = "Admin area. HTML Editor. Manage pictures", SystemName = "HtmlEditor.ManagePictures", Category = "Configuration" };

        public virtual IEnumerable<PermissionRecord> GetEmployerPermissions()
        {
            return new[]
		    {
		        AccessAdminPanel,
                PublicStoreAllowNavigation,
                 //ManageAttributes,
                 ManageCustomers,
                 ManageContributionBenefit,
                 ManageContributions,
                 ManageLoans, 
                 ManageBenefits,
                 ManageReturns,
                 ManageReports,
		        ManageSystemLog,
		        ManageMessageQueue,
		        ManageMaintenance,
                ManageReports,
		    };
        }

        public virtual IEnumerable<PermissionRecord> GetManagerPermissions()
        {
            return new[]
		    {
		        AccessAdminPanel,
                PublicStoreAllowNavigation,
                 //ManageAttributes,
                 ManageCustomers,
                 ManageContributionBenefit,
                 ManageContributions,
                 ManageLoans, 
                 ManageBenefits,
                 ManageReturns,
                 ManageReports,
                 ApprovalBenefit,
                 ApprovalLoan,
		        ManageSystemLog,
		        ManageMessageQueue,
		        ManageMaintenance,
                ManageReports,
		    };
        }

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                AccessAdminPanel,
                
                ManageCustomers,
                ManageContributionBenefit,
                ManageContributions,
                ManageLoans,
                ManageBenefits,
                ManageReturns,
                //AllowCustomerImpersonation,
                //ManageProducts,
                //ManageCategories,
                //ManageManufacturers,
                //ManageProductReviews,
                //ManageProductTags,
                //ManageAttributes,
                //ManageVendors,
                //ManageCurrentCarts,
                //ManageOrders,
                //ManageRecurringPayments,
                //ManageGiftCards,
                //ManageReturnRequests,
                //OrderCountryReport,
                //ManageAffiliates,
                //ManageCampaigns,
                //ManageDiscounts,
                //ManageNewsletterSubscribers,
                //ManagePolls,
                //ManageNews,
                //ManageBlog,
                //ManageWidgets,
                //ManageTopics,
                //ManageForums,
                ManageMessageTemplates,
                //ManagePaymentMethods,
                //ManageExternalAuthenticationMethods,
                //ManageTaxSettings,
                //ManageShippingSettings,
                //ManageCurrencies,
                //ManageMeasures,
                ManageAcl,
                //ManagePlugins,
                HtmlEditorManagePictures,
                //DisplayPrices,
                //EnableShoppingCart,
                //EnableWishlist,
                ManageActivityLog,
                ManageTabs,
                ManageCountries,
                ManageLanguages,
                ManageSettings,
                ManageEmailAccounts,
                ManageKsSystem,
                ManageSystemLog,
                ManageMessageQueue,
                ManageMaintenance,
                ManageScheduleTasks,
                ManageScheduleBatchs,
                ManageReports,
                PublicStoreAllowNavigation,
                //AccessClosedStore,
                ApprovalLoan,
                ApprovalBenefit,
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                new DefaultPermissionRecord 
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[] 
                    {
                        AccessAdminPanel,
                        //AllowCustomerImpersonation,
                        //ManageProducts,
                        //ManageCategories,
                        //ManageManufacturers,
                        //ManageProductReviews,
                        //ManageProductTags,
                        //ManageAttributes,
                        ManageCustomers,
                        ManageContributionBenefit,
                        ManageContributions,
                        ManageLoans,
                        ManageBenefits,
                        ManageReturns,
                        ManageReports,
                        //ManageVendors,
                        //ManageCurrentCarts,
                        //ManageOrders,
                        //ManageRecurringPayments,
                        //ManageGiftCards,
                        //ManageReturnRequests,
                        //OrderCountryReport,
                        //ManageAffiliates,
                        //ManageCampaigns,
                        //ManageDiscounts,
                        //ManageNewsletterSubscribers,
                        //ManagePolls,
                        //ManageNews,
                        //ManageBlog,
                        //ManageWidgets,
                        //ManageTopics,
                        ManageTabs,
                        //ManageForums,
                        //ManageMessageTemplates,
                        ManageCountries,
                        ManageLanguages,
                        ManageSettings,
                        //ManagePaymentMethods,
                        //ManageExternalAuthenticationMethods,
                        //ManageTaxSettings,
                        //ManageShippingSettings,
                        //ManageCurrencies,
                        //ManageMeasures,
                        ManageActivityLog,
                        ManageAcl,
                        ManageEmailAccounts,
                        ManageKsSystem,
                        //ManagePlugins,
                        ManageSystemLog,
                        ManageMessageQueue,
                        ManageMaintenance,
                        HtmlEditorManagePictures,
                        ManageScheduleTasks,
                        ManageScheduleBatchs,
                        //DisplayPrices,
                        //EnableShoppingCart,
                        //EnableWishlist,
                        PublicStoreAllowNavigation,
                        //AccessClosedStore
                    }
                },
                 
                new DefaultPermissionRecord 
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Guests,
                    PermissionRecords = new[] 
                    {
                        //DisplayPrices,
                        //EnableShoppingCart,
                        //EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                },
                new DefaultPermissionRecord 
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Manager,
                    PermissionRecords = GetManagerPermissions()
                } ,
                new DefaultPermissionRecord 
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Employee,
                    PermissionRecords = GetEmployerPermissions()
                } 
            };
        }
    }
}