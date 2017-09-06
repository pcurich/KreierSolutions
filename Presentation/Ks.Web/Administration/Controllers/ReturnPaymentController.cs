using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Core;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Messages;
using Ks.Services.Common;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Messages;
using Ks.Services.Security;
using Ks.Web.Framework;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public class ReturnPaymentController : BaseAdminController
    {
        #region Fields

        private readonly IReturnPaymentService _returnPaymentService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly BankSettings _bankSettings;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructor

        public ReturnPaymentController(
            IReturnPaymentService returnPaymentService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            BankSettings bankSettings,
            IWorkContext workContext,
            IWorkFlowService workFlowService)
        {
            _returnPaymentService = returnPaymentService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
            _bankSettings = bankSettings;
            _customerService = customerService;
            _workFlowService = workFlowService;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturns))
                return AccessDeniedView();

            var model = new ReturnPaymentListModel
            {
                States = ReturnPaymentState.Aprobado.ToSelectList().ToList(),
                Types = ReturnPaymentType.Aportacion.ToSelectList().ToList()
            };

            model.Types.Insert(0, new SelectListItem { Text = "-------------------------", Value = "0" });
            model.States.Insert(0, new SelectListItem { Text = "-------------------------", Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ReturnPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturns))
                return AccessDeniedView();

            Customer customer = null;
            if (!string.IsNullOrEmpty(model.SearchAdmCode))
                customer = _customerService.GetCustomerByAdmCode(model.SearchAdmCode);

            if (customer == null && !string.IsNullOrEmpty(model.SearchDni))
                customer = _customerService.GetCustomerByDni(model.SearchDni);

            var returnPayment = _returnPaymentService.SearchReturnPayment(customer != null ? customer.Id : 0,
                model.SearchTypeId, model.PaymentNumber, command.Page - 1, command.PageSize);

            if (returnPayment.Count == 0)
            {
                var gridModel = new DataSourceResult
                {
                    Data = returnPayment.Select(x => x.ToModel()),
                    Total = 1
                };
                return Json(gridModel);
            }
            else
            {
                var gridModel = new DataSourceResult
                {
                    Data = returnPayment.Select(x =>
                    {
                        var toModel = x.ToModel();
                        customer = _customerService.GetCustomerById(x.CustomerId);
                        toModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, TimeZoneInfo.Utc);
                        toModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc, TimeZoneInfo.Utc);
                        toModel.StateName = Enum.GetName(typeof(ReturnPaymentState), x.StateId);
                        toModel.CustomerName = customer.GetFullName();
                        toModel.CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni);
                        toModel.CustomerAdmCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode);
                        toModel.ReturnPaymentTypeName = Enum.GetName(typeof(ReturnPaymentType), x.ReturnPaymentTypeId);
                        return toModel;
                    }),
                    Total = returnPayment.Count()
                };
                return Json(gridModel);
            } 
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturns))
                return AccessDeniedView();

            var returnPayment = _returnPaymentService.GetReturnPaymentById(id);
            if (returnPayment == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            var model = returnPayment.ToModel();
            model.Banks = _bankSettings.PrepareBanks();
            model.States = ReturnPaymentState.Aprobado.ToSelectList().ToList();
            var customer = _customerService.GetCustomerById(returnPayment.CustomerId);
            model.CustomerId = customer.Id;
            model.CustomerAdmCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode);
            model.CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni);
            model.CustomerName = customer.GetFullName();
            model.ReturnPaymentTypeName = Enum.GetName(typeof(ReturnPaymentType), returnPayment.ReturnPaymentTypeId);
            model.StateName = Enum.GetName(typeof(ReturnPaymentState), returnPayment.StateId);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnPayment.CreatedOnUtc, DateTimeKind.Utc);
            model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(returnPayment.UpdatedOnUtc, DateTimeKind.Utc);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(ReturnPaymentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturns))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var returnPayment = _returnPaymentService.GetReturnPaymentById(model.Id);

                //Esto es cuando se pone el cheque y el banco 
                if (!string.IsNullOrEmpty(model.BankName) &&
                    returnPayment.StateId == (int)ReturnPaymentState.Creado &&
                    string.IsNullOrEmpty(returnPayment.BankName))
                {
                    returnPayment = model.ToEntity(returnPayment);
                    returnPayment.BankName = _bankSettings.PrepareBanks().Where(x => x.Value == model.BankName).FirstOrDefault().Text;
                    returnPayment.StateId = (int)ReturnPaymentState.PorAprobar;
                    returnPayment.UpdatedOnUtc = DateTime.UtcNow;
                    _returnPaymentService.UpdateReturnPayment(returnPayment);

                    #region Flow - Approval required

                    _workFlowService.InsertWorkFlow(new WorkFlow
                    {
                        CustomerCreatedId = _workContext.CurrentCustomer.Id,
                        EntityId=model.Id,
                        EntityName =  CommonHelper.GetKsCustomTypeConverter(typeof(ReturnPayment)).ConvertToInvariantString(new ReturnPayment()),
                        RequireCustomer = false,
                        RequireSystemRole = true,
                        SystemRoleApproval = SystemCustomerRoleNames.Manager,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                        Active = true,
                        Title = "Aprobar devolución",
                        Description = "Se requiere aprobacion para la emision del cheque N° " + model.CheckNumber +
                        " del banco " + returnPayment.BankName + " bajo el concepto de devolucion por " + model.ReturnPaymentTypeName,
                        GoTo = "Admin/ReturnPayment/Edit/"+model.Id
                    });
                    #endregion

                    //activity log
                    _customerActivityService.InsertActivity("EditReturnPayment",
                        _localizationService.GetResource("ActivityLog.EditReturnPaymentBegin"), returnPayment.Id, _workContext.CurrentCustomer.GetFullName(),SystemCustomerRoleNames.Manager);

                }

                //esto es cuando se aprueba o se denega el pago
                if (returnPayment.StateId == (int)ReturnPaymentState.PorAprobar &&
                    (model.StateId == (int)ReturnPaymentState.Aprobado ||
                     model.StateId == (int)ReturnPaymentState.Denegado))
                {
                    returnPayment.StateId = model.StateId;
                    returnPayment.UpdatedOnUtc = DateTime.UtcNow;
                    _returnPaymentService.UpdateReturnPayment(returnPayment);

                    #region Flow - Accepted Denied

                    _workFlowService.InsertWorkFlow(new WorkFlow
                    {
                        CustomerCreatedId = _workContext.CurrentCustomer.Id,
                        EntityId = model.Id,
                        EntityName = CommonHelper.GetKsCustomTypeConverter(typeof(ReturnPayment)).ConvertToInvariantString(new ReturnPayment()),
                        RequireCustomer = false,
                        RequireSystemRole = true,
                        SystemRoleApproval = SystemCustomerRoleNames.Employee,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                        Active = true,
                        Title = "Resultado de la devolución N° " + model.PaymentNumber,
                        Description = "Devolución N° " + model.PaymentNumber + " ha sido " + Enum.GetName(typeof(ReturnPaymentState), model.StateId),
                        GoTo = "Admin/ReturnPayment/Edit/"+model.Id
                    });
                    #endregion

                    //activity log
                    _customerActivityService.InsertActivity("EditReturnPayment",
                        _localizationService.GetResource("ActivityLog.EditReturnPaymentMiddle"), returnPayment.Id, _workContext.CurrentCustomer.GetFullName(), SystemCustomerRoleNames.Employee);
                }

                if (model.StateId == (int) ReturnPaymentState.Entregado ||
                    model.StateId == (int) ReturnPaymentState.Cerrado)
                {
                    returnPayment.StateId = model.StateId;
                    _returnPaymentService.UpdateReturnPayment(returnPayment);

                    //activity log
                    _customerActivityService.InsertActivity("EditReturnPayment",
                        _localizationService.GetResource("ActivityLog.EditReturnPaymentClose"), returnPayment.Id, _workContext.CurrentCustomer.GetFullName());
                }

                SuccessNotification(_localizationService.GetResource("Admin.Contract.ReturnPayment.Updated"));

                #region Flow Close

                _workFlowService.CloseWorkFlow(model.Id,
                    CommonHelper.GetKsCustomTypeConverter(typeof(ReturnPayment)).
                    ConvertToInvariantString(new ReturnPayment()), SystemCustomerRoleNames.Employee);

                #endregion

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();
                    return RedirectToAction("Edit", new { id = model.Id });
                }
                return RedirectToAction("List");
            }

            return View(model);
        }

        #endregion


    }
}