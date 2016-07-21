using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Directory;
using Ks.Services.Events;

namespace Ks.Services.Directory
{
    /// <summary>
    ///     Country service
    /// </summary>
    public class CountryService : ICountryService
    {
        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="countryRepository">Country repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CountryService(ICacheManager cacheManager,
            IRepository<Country> countryRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _countryRepository = countryRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Constants

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : show hidden records?
        /// </remarks>
        private const string COUNTRIES_ALL_KEY = "Ks.country.all-{0}";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string COUNTRIES_PATTERN_KEY = "Ks.country.";

        #endregion

        #region Fields

        private readonly IRepository<Country> _countryRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Methods

        /// <summary>
        ///     Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void DeleteCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Delete(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(country);
        }

        /// <summary>
        ///     Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Countries</returns>
        public virtual IList<Country> GetAllCountries(bool showHidden = false)
        {
            var key = string.Format(COUNTRIES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _countryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
                var countries = query.ToList();
                return countries;
            });
        }


        /// <summary>
        ///     Gets a country
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryById(int countryId)
        {
            if (countryId == 0)
                return null;

            return _countryRepository.GetById(countryId);
        }

        /// <summary>
        ///     Get countries by identifiers
        /// </summary>
        /// <param name="countryIds">Country identifiers</param>
        /// <returns>Countries</returns>
        public virtual IList<Country> GetCountriesByIds(int[] countryIds)
        {
            if (countryIds == null || countryIds.Length == 0)
                return new List<Country>();

            var query = from c in _countryRepository.Table
                where countryIds.Contains(c.Id)
                select c;
            var countries = query.ToList();
            //sort by passed identifiers
            var sortedCountries = new List<Country>();
            foreach (var id in countryIds)
            {
                var country = countries.Find(x => x.Id == id);
                if (country != null)
                    sortedCountries.Add(country);
            }
            return sortedCountries;
        }

        /// <summary>
        ///     Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByTwoLetterIsoCode(string twoLetterIsoCode)
        {
            if (String.IsNullOrEmpty(twoLetterIsoCode))
                return null;

            var query = from c in _countryRepository.Table
                where c.TwoLetterIsoCode == twoLetterIsoCode
                select c;
            var country = query.FirstOrDefault();
            return country;
        }

        /// <summary>
        ///     Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByThreeLetterIsoCode(string threeLetterIsoCode)
        {
            if (String.IsNullOrEmpty(threeLetterIsoCode))
                return null;

            var query = from c in _countryRepository.Table
                where c.ThreeLetterIsoCode == threeLetterIsoCode
                select c;
            var country = query.FirstOrDefault();
            return country;
        }

        /// <summary>
        ///     Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void InsertCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Insert(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(country);
        }

        /// <summary>
        ///     Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void UpdateCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Update(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(country);
        }

        #endregion
    }
}