using System.Collections.Generic;
using Ks.Core.Domain.Directory;

namespace Ks.Services.Directory
{
    /// <summary>
    ///     City service interface
    /// </summary>
    public partial interface ICityService
    {
        /// <summary>
        ///     Deletes a city
        /// </summary>
        /// <param name="city">The city</param>
        void DeleteCity(City city);

        /// <summary>
        ///     Gets a city
        /// </summary>
        /// <param name="cityId">The city identifier</param>
        /// <returns>City</returns>
        City GetCityById(int cityId);

        /// <summary>
        ///     Gets a city collection by state/province identifier
        /// </summary>
        /// <param name="stateProvinceId">State/Province identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<City> GetCitiesByStateProvinceId(int stateProvinceId, bool showHidden = false);

        /// <summary>
        ///     Gets all cities
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Cities</returns>
        IList<City> GetCities(bool showHidden = false);

        /// <summary>
        ///     Inserts a City
        /// </summary>
        /// <param name="city">City</param>
        void InsertCity(City city);

        /// <summary>
        ///     Updates a City
        /// </summary>
        /// <param name="city">City</param>
        void UpdateCity(City city);
    }
}