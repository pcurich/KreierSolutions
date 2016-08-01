using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.System;
using Ks.Services.Events;

namespace Ks.Services.KsSystems
{
    public partial class KsSystemService:IKsSystemService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string KSSYSTEMS_ALL_KEY = "Ks.system.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : KsSystem ID
        /// </remarks>
        private const string KSSYSTEMS_BY_ID_KEY = "Ks.system.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string KSSYSTEMS_PATTERN_KEY = "Ks.system.";

        #endregion

        #region Fields

        private readonly IRepository<KsSystem> _ksSystemRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="ksSystemRepository">KsSystem repository</param>
        /// <param name="eventPublisher">Event published</param>
        public KsSystemService(ICacheManager cacheManager,
            IRepository<KsSystem> ksSystemRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _ksSystemRepository =  ksSystemRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a store
        /// </summary>
        /// <param name="ksSystem">KsSystem</param>
        public virtual void DeleteKsSystem(KsSystem ksSystem)
        {
            if (ksSystem == null)
                throw new ArgumentNullException("ksSystem");

            var allKsSystem = GetAllKsSystems();
            if (allKsSystem.Count == 1)
                throw new Exception("You cannot delete the only configured ksSystem");

            _ksSystemRepository.Delete(ksSystem);

            _cacheManager.RemoveByPattern(KSSYSTEMS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(ksSystem);
        }

        /// <summary>
        /// Gets all ksSystems  
        /// </summary>
        /// <returns>KsSystems </returns>
        public virtual IList<KsSystem> GetAllKsSystems()
        {
            string key = string.Format(KSSYSTEMS_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _ksSystemRepository.Table
                            orderby  s.Id
                            select s;

                var ksSystems = query.ToList();
                return ksSystems;
            });
        }

        /// <summary>
        /// Gets a ksSystem 
        /// </summary>
        /// <param name="ksSystemId">KsSystem identifier</param>
        /// <returns>Store</returns>
        public virtual KsSystem GetKsSystemById(int ksSystemId)
        {
            if (ksSystemId == 0)
                return null;

            string key = string.Format(KSSYSTEMS_BY_ID_KEY, ksSystemId);
            return _cacheManager.Get(key, () => _ksSystemRepository.GetById(ksSystemId));
        }

        /// <summary>
        /// Inserts a ksSystem
        /// </summary>
        /// <param name="ksSystem">KsSystem</param>
        public virtual void InsertKsSystem(KsSystem ksSystem)
        {
            if (ksSystem == null)
                throw new ArgumentNullException("ksSystem");

            _ksSystemRepository.Insert(ksSystem);

            _cacheManager.RemoveByPattern(KSSYSTEMS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(ksSystem);
        }

        /// <summary>
        /// Updates the ksSystem
        /// </summary>
        /// <param name="ksSystem">KsSystem</param>
        public virtual void UpdateKsSystem(KsSystem ksSystem)
        {
            if (ksSystem == null)
                throw new ArgumentNullException("ksSystem");

            _ksSystemRepository.Update(ksSystem);

            _cacheManager.RemoveByPattern(KSSYSTEMS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(ksSystem);
        }

        #endregion
    }
}