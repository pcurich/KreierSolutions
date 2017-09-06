using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Core;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Services.Contract;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Security;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public class CheckController : BaseAdminController
    {
        #region Fields

        private readonly ICheckService _checkService;
        private readonly IPermissionService _permissionService;
        private readonly BankSettings _bankSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        private readonly ILoanService _loanService;
        private readonly IBenefitService _benefitService;
        private readonly IReturnPaymentService _returnPaymentService;

        #endregion

        #region Constructor

        public CheckController(ICheckService checkService, IPermissionService permissionService, BankSettings bankSettings, ILocalizationService localizationService, ICustomerActivityService customerActivityService, IDateTimeHelper dateTimeHelper, IWorkContext workContext, ILoanService loanService, IBenefitService benefitService, IReturnPaymentService returnPaymentService)
        {
            _checkService = checkService;
            _permissionService = permissionService;
            _bankSettings = bankSettings;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
            _loanService = loanService;
            _benefitService = benefitService;
            _returnPaymentService = returnPaymentService;
        }

        #endregion

        #region Methods
        // GET: Check
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChecks))
                return AccessDeniedView();
            var checkListModel = new CheckListModel();
            checkListModel.Entities.Add(new SelectListItem
            {
                Value = "0",
                Text = "---------------"
            });
            checkListModel.Entities.Add(new SelectListItem
            {
                Value = CommonHelper.GetKsCustomTypeConverter(typeof(ReturnPayment)).ConvertToInvariantString(new ReturnPayment()),
                Text = "Devoluciones"
            });
            checkListModel.Entities.Add(new SelectListItem
            {
                Value = CommonHelper.GetKsCustomTypeConverter(typeof(Loan)).ConvertToInvariantString(new Loan()),
                Text = "Apoyo Social Económico"
            });
            checkListModel.Entities.Add(new SelectListItem
            {
                Value = CommonHelper.GetKsCustomTypeConverter(typeof(ContributionBenefitBank)).ConvertToInvariantString(new ContributionBenefitBank()),
                Text = "Beneficios"
            });
            checkListModel.Banks = _bankSettings.PrepareBanks();
            return View(checkListModel);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, CheckListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChecks))
                return AccessDeniedView();

            DateTime? dateFrom =null;
            DateTime? dateTo = null;

            if(model.SearchFrom.HasValue)
              dateFrom = _dateTimeHelper.ConvertToUtcTime(model.SearchFrom.Value, DateTimeKind.Local);

            if (model.SearchTo.HasValue)
            {
                dateTo = _dateTimeHelper.ConvertToUtcTime(model.SearchTo.Value, DateTimeKind.Local);
                dateTo = dateTo.Value.AddSeconds(86399);
            }
            
            var list = _checkService.SearchCheck(dateFrom, dateTo,
                model.EntityName,
                model.BankName.Contains("----") ? null : model.BankName,
                model.CheckNumber,
                command.Page - 1, command.PageSize);

            var tomodel = list.Select(x =>
            {
                var temp = x.ToModel();
                temp.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                var checkState =
                    Web.Framework.Extensions.GetDescription(typeof(CheckSatate),
                        Convert.ToString(x.CheckStateId));
                temp.CheckStateName = checkState;
                var entityName =
                    Web.Framework.Extensions.GetDescription(typeof(EntityTypeValues), Convert.ToString(x.EntityTypeId));
                temp.EntityName = entityName;
                if (x.CheckStateId == 2)
                    temp.EntityId = 0;
                return temp;
            });

            var gridModel = new DataSourceResult
            {
                Data = tomodel,
                Total = list.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Edit(int entityId, int entityTypeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChecks))
                return AccessDeniedView();

            if (entityId == 0)
            {
                ErrorNotification("No se puede cambiar informacion histórica");
                return RedirectToAction("List");
            }
            var model = new CheckCompareModel();

            switch (entityTypeId)
            {
                case (int)EntityTypeValues.Loan:
                    var loan = _loanService.GetLoanById(entityId);
                    if (loan == null)
                        break;
                    model.After = new CheckModel
                    {
                        AccountNumber = loan.AccountNumber,
                        Amount = loan.TotalAmount,
                        BankName = loan.BankName,
                        CheckNumber = loan.CheckNumber,
                        CheckStateName = "Vigente",
                        CheckStateId = (int)CheckSatate.Active,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(loan.CreatedOnUtc, DateTimeKind.Utc),
                        EntityId = entityId,
                        EntityTypeId = entityTypeId,
                        EntityName = "Apoyo Social Económico",
                    };
                    break;
                case (int)EntityTypeValues.Benefit:
                    var benefit = _benefitService.GetContributionBenefitBankById(entityId);
                    if (benefit == null)
                        break;
                    model.After = new CheckModel
                    {
                        AccountNumber = benefit.AccountNumber,
                        Amount = benefit.AmountToPay,
                        BankName = benefit.BankName,
                        CheckNumber = benefit.CheckNumber,
                        CheckStateName = "Vigente",
                        CheckStateId = (int)CheckSatate.Active,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(benefit.CreatedOnUtc, DateTimeKind.Utc),
                        EntityId = entityId,
                        EntityTypeId = entityTypeId,
                        EntityName = "Beneficio",
                    };
                    break;
                case (int)EntityTypeValues.Return:
                    var retur = _returnPaymentService.GetReturnPaymentById(entityId);
                    if (retur == null)
                        break;
                    model.After = new CheckModel
                    {
                        AccountNumber = retur.AccountNumber,
                        Amount = retur.AmountToPay,
                        BankName = retur.BankName,
                        CheckNumber = retur.CheckNumber,
                        CheckStateName = "Vigente",
                        CheckStateId = (int)CheckSatate.Active,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(retur.CreatedOnUtc, DateTimeKind.Utc),
                        EntityId = entityId,
                        EntityTypeId = entityTypeId,
                        EntityName = "Devolución",
                    };
                    break;
                default:
                    break;
            }

            if (model.After == null)
                model.After = new CheckModel();

            model.After.Banks = _bankSettings.PrepareBanks();
            model.Before = new CheckModel { Banks = _bankSettings.PrepareBanks() };

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(CheckCompareModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChecks))
                return AccessDeniedView();

            if (model.After.CheckStateId == (int)CheckSatate.Cancel)
            {
                ErrorNotification("No se puede cambiar una informacion que es historica");
                return RedirectToAction("List");
            }
            else
            {
                if (string.IsNullOrEmpty(model.Before.CheckNumber))
                {
                    ErrorNotification("Ingrese un numero de cheque");
                    return View(model);
                }
                else
                {
                    var chek = new Check();

                    switch (model.After.EntityTypeId)
                    {
                        case (int)EntityTypeValues.Loan:
                            #region Replace Loan

                            var loan = _loanService.GetLoanById(model.After.EntityId);
                            chek.AccountNumber = string.IsNullOrEmpty(loan.AccountNumber) ? "NO DATA" : loan.AccountNumber;
                            chek.BankName = string.IsNullOrEmpty(loan.BankName) ? "NO DATA" : loan.BankName;
                            chek.CheckNumber = string.IsNullOrEmpty(loan.CheckNumber) ? "NO DATA" : loan.CheckNumber;
                            chek.Amount = loan.TotalAmount;
                            chek.CreatedOnUtc = DateTime.UtcNow;
                            chek.CheckStateId = (int)CheckSatate.Cancel;
                            chek.EntityId = model.After.EntityId;
                            chek.EntityName = CommonHelper.GetKsCustomTypeConverter(typeof(Loan)).ConvertToInvariantString(new Loan());
                            chek.EntityTypeId = (int)EntityTypeValues.Loan;
                            chek.Reason = model.Before.Reason;

                            loan.BankName = _bankSettings.GetBanKName(model.Before.BankName);
                            loan.AccountNumber = model.Before.AccountNumber;
                            loan.CheckNumber = model.Before.CheckNumber;
                            loan.ApprovalOnUtc = DateTime.UtcNow;

                            _checkService.Replace(loan, chek);

                            #endregion
                            break;
                        case (int)EntityTypeValues.Benefit:
                            #region Replace Benefit
                            var benefit = _benefitService.GetContributionBenefitBankById(model.After.EntityId);
                            chek.AccountNumber = string.IsNullOrEmpty(benefit.AccountNumber) ? "NO DATA" : benefit.AccountNumber;
                            chek.BankName = string.IsNullOrEmpty(benefit.BankName) ? "NO DATA" : benefit.BankName;
                            chek.CheckNumber = string.IsNullOrEmpty(benefit.CheckNumber) ? "NO DATA" : benefit.CheckNumber;
                            chek.Amount = benefit.AmountToPay;
                            chek.CreatedOnUtc = DateTime.UtcNow;
                            chek.CheckStateId = (int)CheckSatate.Cancel;
                            chek.EntityId = model.After.EntityId;
                            chek.EntityName = CommonHelper.GetKsCustomTypeConverter(typeof(ContributionBenefitBank)).ConvertToInvariantString(new ContributionBenefitBank());
                            chek.EntityTypeId = (int)EntityTypeValues.Benefit;
                            chek.Reason = model.Before.Reason;

                            benefit.BankName = _bankSettings.GetBanKName(model.Before.BankName);
                            benefit.AccountNumber = model.Before.AccountNumber;
                            benefit.CheckNumber = model.Before.CheckNumber;
                            benefit.ApprovedOnUtc = DateTime.UtcNow;

                            _checkService.Replace(benefit, chek);
                            #endregion
                            break;
                        case (int)EntityTypeValues.Return:
                            #region Replace Return
                            var retur = _returnPaymentService.GetReturnPaymentById(model.After.EntityId);
                            chek.AccountNumber = string.IsNullOrEmpty(retur.AccountNumber) ? "NO DATA" : retur.AccountNumber;
                            chek.BankName = string.IsNullOrEmpty(retur.BankName) ? "NO DATA" : retur.BankName;
                            chek.CheckNumber = string.IsNullOrEmpty(retur.CheckNumber) ? "NO DATA" : retur.CheckNumber;
                            chek.Amount = retur.AmountToPay;
                            chek.CreatedOnUtc = DateTime.UtcNow;
                            chek.CheckStateId = (int)CheckSatate.Cancel;
                            chek.EntityId = model.After.EntityId;
                            chek.EntityName = CommonHelper.GetKsCustomTypeConverter(typeof(ReturnPayment)).ConvertToInvariantString(new ReturnPayment());
                            chek.EntityTypeId = (int)EntityTypeValues.Benefit;
                            chek.Reason = model.Before.Reason;

                            retur.BankName = _bankSettings.GetBanKName(model.Before.BankName);
                            retur.AccountNumber = model.Before.AccountNumber;
                            retur.CheckNumber = model.Before.CheckNumber;
                            retur.CreatedOnUtc = DateTime.UtcNow;

                            _checkService.Replace(retur, chek);
                            #endregion
                            break;
                        default: break;
                    }

                    SuccessNotification(_localizationService.GetResource("Cheque correctamente cambiado"));
                    if (continueEditing)
                    {
                        return RedirectToAction("Edit", new { entityId = model.After.EntityId, entityTypeId = model.After.EntityTypeId });
                    }
                    return RedirectToAction("List");
                }
            }

        }

        #endregion
    }
}