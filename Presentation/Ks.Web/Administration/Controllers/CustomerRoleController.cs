using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Customers;
using Ks.Core;
using Ks.Core.Domain.Customers;
using Ks.Services.Common;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Security;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public partial class CustomerRoleController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IKsSystemService _ksSystemService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public CustomerRoleController(ICustomerService customerService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper, 
            IKsSystemService ksSystemService,
            IWorkContext workContext)
        {
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._ksSystemService =  ksSystemService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected CustomerRoleModel PrepareCustomerRoleModel(CustomerRole customerRole)
        {
            var model = customerRole.ToModel();
            //var product = _productService.GetProductById(customerRole.PurchasedWithProductId);
            //if (product != null)
            //{
            //    model.PurchasedWithProductName = product.Name;
            //}
            return model;
        }

        [NonAction]
        protected virtual CustomerModel PrepareCustomerModelForList(Customer customer)
        {
            return new CustomerModel
            {
                Id = customer.Id,
                Email = customer.Email,
                Username = customer.Username,
                FullName = customer.GetFullName(),
                AdmCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                Dni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni),
                Active = customer.Active,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc),
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc),
            };
        }
        #endregion

        #region Customer roles

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customerRoles = _customerService.GetAllCustomerRoles(true);
            var gridModel = new DataSourceResult
            {
                Data = customerRoles.Select(PrepareCustomerRoleModel),
                Total = customerRoles.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var model = new CustomerRoleModel {Active = true};
            //default values
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var customerRole = model.ToEntity();
                _customerService.InsertCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerRole", _localizationService.GetResource("ActivityLog.AddNewCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            var model = PrepareCustomerRoleModel(customerRole);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customerRole = _customerService.GetCustomerRoleById(model.Id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (customerRole.IsSystemRole && !model.Active)
                        throw new KsException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                    if (customerRole.IsSystemRole && !customerRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new KsException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));

                    //if (SystemCustomerRoleNames.Registered.Equals(customerRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                    //    model.PurchasedWithProductId > 0)
                    //    throw new KsException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Registered"));

                    customerRole = model.ToEntity(customerRole);
                    _customerService.UpdateCustomerRole(customerRole);

                    //activity log
                    _customerActivityService.InsertActivity("EditCustomerRole", _localizationService.GetResource("ActivityLog.EditCustomerRole"), customerRole.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
                }

                //If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                _customerService.DeleteCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomerRole", _localizationService.GetResource("ActivityLog.DeleteCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }

        }

        [HttpPost]
        public ActionResult CustomerRoleList(DataSourceRequest command, CustomerRoleListModel model)
        {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomerRoles))
                return AccessDeniedView();

            var customers = _customerService.GetCustomerByCustomerRole(model.CustomerRoleId, model.CustomerDni, model.CustomerAdminCode,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            if (model.IsNew && customers.Count == 0 )
            {
                Customer customer = null;
                if(model.CustomerDni != null && model.CustomerDni.Length > 0)
                {
                    customer = _customerService.GetCustomerByDni(model.CustomerDni);
                }

                if (model.CustomerAdminCode!= null && model.CustomerAdminCode.Length > 0)
                {
                    customer = _customerService.GetCustomerByAdmCode(model.CustomerAdminCode);
                }

                if(customer != null)
                {
                    customer.CustomerRoles.Add(_customerService.GetCustomerRoleById(model.CustomerRoleId)); 
                    _customerService.UpdateCustomer(customer);
                }

                customers.Add(customer);
            }

            
            var gridModel = new DataSourceResult
            {
                Data = customers.Select(PrepareCustomerModelForList),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

 
        public ActionResult DeleteCustomerRole(int customerId, int customerRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (customerId == 0 || customerRoleId == 0)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                var customer = _customerService.GetCustomerById(customerId);
                _customerService.DeleteCustomerInRole(customerId, customerRoleId);

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomerRole", _localizationService.GetResource("ActivityLog.DeleteCustomerRole"), customer.GetFullName());

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Deleted"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                
            }
            return RedirectToAction("Edit", new { id = customerRoleId });
        }

        #endregion
    }
}