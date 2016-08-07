using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Directory;
using Ks.Services.Events;
using Ks.Services.Localization;

namespace Ks.Services.Directory
{
    public   partial class CityService : ICityService 
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : language ID
        /// {2} : show hidden records?
        /// </remarks>
        private const string CITIES_ALL_KEY = "Ks.cities.all-{0}-{1}-{2}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CITIES_PATTERN_KEY = "Ks.cities.";

        #endregion

        #region Fields

        private readonly IRepository<City> _cityRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public CityService(IRepository<City> cityRepository, IEventPublisher eventPublisher, ICacheManager cacheManager)
        {
            _cityRepository = cityRepository;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a city
        /// </summary>
        /// <param name="city">The city</param>
        public virtual void DeleteCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            _cityRepository.Delete(city);

            _cacheManager.RemoveByPattern(CITIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(city);
        }

        /// <summary>
        /// Gets a city
        /// </summary>
        /// <param name="cityId">The city identifier</param>
        /// <returns>City</returns>
        public virtual City GetCityById(int cityId)
        {
            if (cityId == 0)
                return null;

            return _cityRepository.GetById(cityId);
        }

        /// <summary>
        /// Gets a city collection by state/province identifier
        /// </summary>
        /// <param name="stateProvinceId">State/Province identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        public virtual IList<City> GetCitiesByStateProvinceId(int stateProvinceId, int languageId = 0, bool showHidden = false)
        {
            string key = string.Format(CITIES_ALL_KEY, stateProvinceId, languageId,showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from sp in _cityRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.StateProvinceId == stateProvinceId &&
                            (showHidden || sp.Published)
                            select sp;
                var cities = query.ToList();

                if (languageId > 0)
                {
                    //we should sort states by localized names when they have the same display order
                    cities = cities
                        .OrderBy(c => c.DisplayOrder)
                        .ThenBy(c => c.GetLocalized(x => x.Name, languageId))
                        .ToList();
                }
                return cities;

            });
        }

        /// <summary>
        /// Gets all cities
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Cities</returns>
        public virtual IList<City> GetCities(bool showHidden = false)
        {
            var query = from sp in _cityRepository.Table
                        orderby sp.StateProvinceId, sp.DisplayOrder, sp.Name
                        where showHidden || sp.Published
                        select sp;
            var cities = query.ToList();
            return cities;
        }

        /// <summary>
        /// Inserts a city
        /// </summary>
        /// <param name="city">City</param>
        public virtual void InsertCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            _cityRepository.Insert(city);

            _cacheManager.RemoveByPattern(CITIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(city);
        }

        /// <summary>
        /// Update a city
        /// </summary>
        /// <param name="city">ity</param>
        public virtual void UpdateCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            _cityRepository.Update(city);

            _cacheManager.RemoveByPattern(CITIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(city);
        }

        #endregion
    }
}