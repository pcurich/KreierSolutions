using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Admin.Models.Customers;
using Ks.Core;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Services.Common;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.Security;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public partial class ContributionsController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IContributionService _contributionService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Constructors

        public ContributionsController(IPermissionService permissionService, IContributionService contributionService, ICustomerService customerService, IGenericAttributeService genericAttributeService, IDateTimeHelper dateTimeHelper)
        {
            _permissionService = permissionService;
            _contributionService = contributionService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
            //    return AccessDeniedView();

            var model = new ContributionListModel
            {
                SearchAdmCode = "",
                SearchDni = "",
                SearchDateCreatedFrom = DateTime.Today.AddMonths(-1),
                SearchDateCreatedTo = DateTime.Today,
                SearchLetter = 0,
                IsActive = true

            };
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ContributionListModel model)
        {
            GenericAttribute generic = null;

            //1) Find By Dni and AdmCode

            if (!string.IsNullOrWhiteSpace(model.SearchDni))
                generic = _genericAttributeService.GetAttributeForKeyValue("Dni", model.SearchDni.Trim());

            if (!string.IsNullOrWhiteSpace(model.SearchAdmCode) && generic == null)
                generic = _genericAttributeService.GetAttributeForKeyValue("AdmCode", model.SearchAdmCode.Trim());

            IPagedList<Contribution> contributions = null;
            if (generic != null && string.Compare(generic.KeyGroup, "Customer", StringComparison.CurrentCulture) == 0)
                contributions = _contributionService.SearchContributionByCustomerId(generic.EntityId, model.IsActive);

            //2) Find by letter Number
            if (contributions == null && model.SearchLetter >= 0)
                contributions = _contributionService.SearchContributionByLetterNumber(model.SearchLetter, model.IsActive);

            //3) Find by createdTime
            if (contributions == null && model.SearchDateCreatedFrom.HasValue && model.SearchDateCreatedTo.HasValue &&
                model.SearchDateCreatedTo.Value.Millisecond > model.SearchDateCreatedFrom.Value.Millisecond)
            {
                var dateFromUtc = _dateTimeHelper.ConvertToUtcTime(model.SearchDateCreatedFrom.Value);
                var dateToUtc = _dateTimeHelper.ConvertToUtcTime(model.SearchDateCreatedTo.Value);
                contributions = _contributionService.SearchContibutionByCreatedOnUtc(dateFromUtc, dateToUtc, model.IsActive);
            }
            if (contributions == null)
                contributions = new PagedList<Contribution>(new List<Contribution>(), 0, 10);

            var contributionsModel = contributions.Select(x =>
            {
                var toModel = x.ToModel();
                toModel.CustomerCompleteName = x.Customer.GetFullName();
                toModel.CustomerDni = x.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni);
                toModel.CustomerAdmCode = x.Customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode);
                return toModel;
            });

            var gridModel = new DataSourceResult
            {
                Data = contributionsModel,
                Total = contributions.TotalCount
            };

            return Json(gridModel);
        }


        #endregion
    }
}