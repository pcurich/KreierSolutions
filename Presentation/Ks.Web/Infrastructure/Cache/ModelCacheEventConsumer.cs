using Ks.Core.Caching;
using Ks.Core.Domain.Catalog;
using Ks.Core.Domain.Configuration;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Localization;
using Ks.Core.Events;
using Ks.Core.Infrastructure;
using Ks.Services.Events;

namespace Ks.Web.Infrastructure.Cache
{
    public class ModelCacheEventConsumer:
        //languages
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>,
        //currencies
        IConsumer<EntityInserted<Currency>>,
        IConsumer<EntityUpdated<Currency>>,
        IConsumer<EntityDeleted<Currency>>,
        //settings
        IConsumer<EntityUpdated<Setting>>,
        //specification attributes
        IConsumer<EntityUpdated<SpecificationAttribute>>,
        IConsumer<EntityDeleted<SpecificationAttribute>>,
        //specification attribute options
        IConsumer<EntityUpdated<SpecificationAttributeOption>>,
        IConsumer<EntityDeleted<SpecificationAttributeOption>>,
        //states/province
        IConsumer<EntityInserted<StateProvince>>,
        IConsumer<EntityUpdated<StateProvince>>,
        IConsumer<EntityDeleted<StateProvince>>,
        //cities
        IConsumer<EntityInserted<City>>,
        IConsumer<EntityUpdated<City>>,
        IConsumer<EntityDeleted<City>>
    {

        /// <summary>
        /// Key for available currencies
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {0} : current System ID
        /// </remarks>
        public const string AVAILABLE_CURRENCIES_MODEL_KEY = "Ks.pres.currencies.all-{0}-{1}";
        public const string AVAILABLE_CURRENCIES_PATTERN_KEY = "Ks.pres.currencies";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : current KsSystem ID
        /// </remarks>
        public const string AVAILABLE_LANGUAGES_MODEL_KEY = "Ks.pres.languages.all-{0}";
        public const string AVAILABLE_LANGUAGES_PATTERN_KEY = "Ks.pres.languages";

        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : "empty" or "select" item
        /// {2} : language ID
        /// </remarks>
        public const string STATEPROVINCES_BY_COUNTRY_MODEL_KEY = "Ks.pres.stateprovinces.bycountry-{0}-{1}-{2}";
        public const string STATEPROVINCES_PATTERN_KEY = "Ks.pres.stateprovinces";


        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : stateprovince ID
        /// {1} : "empty" or "select" item
        /// {2} : language ID
        /// </remarks>
        public const string CITY_BY_STATEPROVINCES_MODEL_KEY = "Ks.pres.city.bystateprovince-{0}-{1}-{2}";
        public const string CITY_PATTERN_KEY = "Ks.pres.cities";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SITEMAP_PAGE_MODEL_KEY = "Ks.pres.sitemap.page-{0}-{1}-{2}";
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SITEMAP_SEO_MODEL_KEY = "Ks.pres.sitemap.seo-{0}-{1}-{2}";
        public const string SITEMAP_PATTERN_KEY = "Ks.pres.sitemap";

        private readonly ICacheManager _cacheManager;

        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("ks_cache_static");
        }

        #region currencies

        public void HandleEvent(EntityInserted<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }

        #endregion

        #region Language

        //languages
        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CITY_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY); //depends on distinct sitemap settings
        }

        public void HandleEvent(EntityUpdated<SpecificationAttribute> eventMessage)
        {
            throw new System.NotImplementedException();
        }

        public void HandleEvent(EntityDeleted<SpecificationAttribute> eventMessage)
        {
            throw new System.NotImplementedException();
        }

        public void HandleEvent(EntityUpdated<SpecificationAttributeOption> eventMessage)
        {
            throw new System.NotImplementedException();
        }

        public void HandleEvent(EntityDeleted<SpecificationAttributeOption> eventMessage)
        {
            throw new System.NotImplementedException();
        }

        #region State/province

        public void HandleEvent(EntityInserted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        #endregion

        #region City

        public void HandleEvent(EntityInserted<City> eventMessage)
        {
             _cacheManager.RemoveByPattern(CITY_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdated<City> eventMessage)
        {
             _cacheManager.RemoveByPattern(CITY_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeleted<City> eventMessage)
        {
             _cacheManager.RemoveByPattern(CITY_PATTERN_KEY);
        }

        #endregion

        #endregion
    }
}