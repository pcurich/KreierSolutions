using System;
using System.Linq;
using System.Web.Mvc;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Services.Directory;
using Ks.Services.Localization;
using Ks.Web.Framework;
using Ks.Web.Infrastructure.Cache;

namespace Ks.Web.Controllers
{
    public partial class StatesProvincesController : BasePublicController
    {
         #region Fields

        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public StatesProvincesController(IStateProvinceService stateProvinceService,
            ICityService cityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._stateProvinceService = stateProvinceService;
            this._cityService = cityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Cities

        //available even when navigation is not allowed
        [PublicKsSystemAllowNavigation(true)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCitiesByStateId(string stateProvinceId, bool addSelectCityItem)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(stateProvinceId))
                throw new ArgumentNullException("stateProvinceId");

            string cacheKey = string.Format(ModelCacheEventConsumer.CITY_BY_STATEPROVINCES_MODEL_KEY, stateProvinceId, addSelectCityItem, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var state = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateProvinceId));
                var cities = _cityService.GetCitiesByStateProvinceId(state != null ? state.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = (from s in cities
                              select new { id = s.Id, name = s.GetLocalized(x => x.Name) })
                              .ToList();


                if (state == null)
                {
                    //state is not selected ("choose state" item)
                    if (addSelectCityItem)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectCity") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.Other") });
                    }
                }
                else
                {
                    //some state is selected
                    if (result.Count == 0)
                    {
                        //state does not have cities
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.Other") });
                    }
                    else
                    {
                        //state has some cities
                        if (addSelectCityItem)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectCity") });
                        }
                    }
                }

                return result;
            });

            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}