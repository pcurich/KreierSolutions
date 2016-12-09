using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Contract;
using Ks.Data;
using Ks.Services.Events;

namespace Ks.Services.Contract
{
    public class BenefitService : IBenefitService
    {
        #region Constants

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : Id
        /// </remarks>
        private const string BENEFITS_BY_KEY = "ks.benefits.{0}";

        /// <summary>
        ///     Key for caching
        /// </summary>
        private const string BENEFITS_ALL = "ks.benefits.all";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string BENEFITS_PATTERN_KEY = "ks.benefits.";

        #endregion

        #region Fields

        private readonly IRepository<Benefit> _benefitRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Constructor

        public BenefitService(IRepository<Benefit> benefitRepository, ICacheManager cacheManager, IEventPublisher eventPublisher, IDataProvider dataProvider, IDbContext dbContext)
        {
            _benefitRepository = benefitRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        public virtual void DeleteBenefit(Benefit benefit)
        {
            if (benefit == null)
                throw new ArgumentNullException("benefit");

            _benefitRepository.Delete(benefit);

            //cache
            _cacheManager.RemoveByPattern(BENEFITS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(benefit);
        }

        public virtual void InsertBenefit(Benefit benefit)
        {
            if (benefit == null)
                throw new ArgumentNullException("benefit");

            _benefitRepository.Insert(benefit);

            //cache
            _cacheManager.RemoveByPattern(BENEFITS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(benefit);
        }

        public virtual void UpdateBenefit(Benefit benefit)
        {
            if (benefit == null)
                throw new ArgumentNullException("benefit");

            _benefitRepository.Update(benefit);

            //cache
            _cacheManager.RemoveByPattern(BENEFITS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(benefit);
        }

        public virtual Benefit GetBenefitById(int benefitId)
        {
            string key = string.Format(BENEFITS_BY_KEY, benefitId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _benefitRepository.Table
                            orderby cr.Id
                            where cr.Id == benefitId
                            select cr;
                var benefit = query.FirstOrDefault();
                return benefit;
            });
        }

        public virtual List<Benefit> GetAllBenefits()
        {
            string key = string.Format(BENEFITS_ALL);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _benefitRepository.Table
                            orderby cr.Id
                            select cr;
                var benefit = query.ToList();
                return benefit;
            });
        }

        public virtual IPagedList<Benefit> SearchBenefits(int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = GetAllBenefits();
            return new PagedList<Benefit>(query, pageIndex, pageSize);
        }

        #endregion
    }
}