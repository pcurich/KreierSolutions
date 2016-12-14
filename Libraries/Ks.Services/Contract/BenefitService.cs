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

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : customerId
        ///     {1} : contributionId
        /// </remarks>
        private const string CONTRIBUTIONBENEFIS_BY_KEY = "ks.contributionbenefit.{0}.{1}";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string CONTRIBUTIONBENEFITS_PATTERN_KEY = "ks.contributionbenefit.";

        #endregion

        #region Fields

        private readonly IRepository<Benefit> _benefitRepository;
        private readonly IRepository<Contribution> _contributionRepository;
        private readonly IRepository<ContributionBenefit> _contributionBenefitRepository;
        private readonly IRepository<ContributionBenefitBank> _contributionBenefitBankRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Constructor

        public BenefitService(
            IRepository<Benefit> benefitRepository,
            IRepository<Contribution> contributionRepository,
            IRepository<ContributionBenefit> contributionBenefitRepository,
            IRepository<ContributionBenefitBank> contributionBenefitBankRepository,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IDataProvider dataProvider,
            IDbContext dbContext)
        {
            _benefitRepository = benefitRepository;
            _contributionRepository = contributionRepository;
            _contributionBenefitRepository = contributionBenefitRepository;
            _contributionBenefitBankRepository = contributionBenefitBankRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        #region Benefit

        public virtual void DeleteBenefit(Benefit benefit)
        {
            if (benefit == null)
                throw new ArgumentNullException("benefit");

            _benefitRepository.Delete(benefit);

            //cache
            _cacheManager.RemoveByPattern(BENEFITS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(benefit);
        }

        public virtual void InsertBenefit(Benefit benefit)
        {
            if (benefit == null)
                throw new ArgumentNullException("benefit");

            _benefitRepository.Insert(benefit);

            //cache
            _cacheManager.RemoveByPattern(BENEFITS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(benefit);
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

        public virtual List<Benefit> GetActiveBenefits()
        {
            string key = string.Format(BENEFITS_ALL);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _benefitRepository.Table
                            where cr.IsActive
                            orderby cr.Id
                            select cr;
                var benefit = query.ToList();
                return benefit;
            });
        }

        public virtual IPagedList<Benefit> GetAllBenefits(int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from cr in _benefitRepository.Table
                        orderby cr.Id
                        select cr;
            var benefit = query.ToList();

            return new PagedList<Benefit>(benefit, pageIndex, pageSize);
        }

        #endregion

        #region ContributionBenefit

        public virtual void DeleteContributionBenefit(ContributionBenefit contributionBenefit)
        {
            if (contributionBenefit == null)
                throw new ArgumentNullException("contributionBenefit");

            _contributionBenefitRepository.Delete(contributionBenefit);

            //cache
            _cacheManager.RemoveByPattern(CONTRIBUTIONBENEFITS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(contributionBenefit);
        }

        public virtual void InsertContributionBenefit(ContributionBenefit contributionBenefit)
        {
            if (contributionBenefit == null)
                throw new ArgumentNullException("contributionBenefit");

            _contributionBenefitRepository.Insert(contributionBenefit);

            //cache
            _cacheManager.RemoveByPattern(CONTRIBUTIONBENEFITS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(contributionBenefit);
        }

        public virtual void UpdateContributionBenefit(ContributionBenefit contributionBenefit)
        {
            if (contributionBenefit == null)
                throw new ArgumentNullException("contributionBenefit");

            _contributionBenefitRepository.Update(contributionBenefit);

            //cache
            _cacheManager.RemoveByPattern(CONTRIBUTIONBENEFITS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(contributionBenefit);
        }

        public virtual ContributionBenefit GetContributionBenefitbyId(int contributionBenefitId)
        {
            var query = from b in _contributionBenefitRepository.Table
                        where b.Id == contributionBenefitId
                        select b;

            return query.FirstOrDefault();
        }

        public virtual IPagedList<ContributionBenefit> GetAllContributionBenefitByCustomer(int customerId = 0, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {

            var query = from b in _contributionBenefitRepository.Table
                        join c in _contributionRepository.Table on b.ContributionId equals c.Id
                        where c.CustomerId == customerId && c.Active
                        select b;

            return new PagedList<ContributionBenefit>(query.ToList(), pageIndex, pageSize);
        }

        public virtual List<ContributionBenefit> GetContributionBenefitsByCustomer(int customerId)
        {
            string key = string.Format(CONTRIBUTIONBENEFIS_BY_KEY, customerId, 0);
            return _cacheManager.Get(key, () =>
            {
                var query = (from cb in _contributionBenefitRepository.Table
                             join cc in _contributionRepository.Table on cb.ContributionId equals cc.Id
                             where cc.CustomerId == customerId
                             select cb);

                return query.ToList();
            });
        }

        public virtual ContributionBenefit GetContributionBenefitsByContribution(int customerId, int contributionId)
        {
            string key = string.Format(CONTRIBUTIONBENEFIS_BY_KEY, customerId, contributionId);
            return _cacheManager.Get(key, () =>
            {
                var query = (from cb in _contributionBenefitRepository.Table
                             join cc in _contributionRepository.Table on cb.ContributionId equals cc.Id
                             where cc.CustomerId == customerId
                             select cb);

                return query.FirstOrDefault();
            });
        }

        #endregion

        #endregion
    }
}