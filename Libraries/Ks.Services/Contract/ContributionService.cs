using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Services.Events;
using MaxMind.GeoIP2.Model;

namespace Ks.Services.Contract
{
    public class ContributionService : IContributionService
    {
        #region Constructor

        public ContributionService(IRepository<Customer> customerRepository,
            IRepository<Contribution> contributionRepository, ICacheManager cacheManager, IEventPublisher eventPublisher)
        {
            _customerRepository = customerRepository;
            _contributionRepository = contributionRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        public virtual void DeleteContribution(Contribution contribution)
        {
            if (contribution == null)
                throw new ArgumentNullException("contribution");

            contribution.Active = false;
            UpdateContribution(contribution);
        }

        public virtual List<Contribution> GetContributionGroupByDelay()
        {
            var query = from c in _contributionRepository.Table
                where c.IsDelay 
                group c by new { c.CycleOfDelay } into temp
                select new  
                {
                    CycleOfDelay = temp.Key,
                    AmountTotal = temp.Count()
                };

            var contributions = query.ToList();

            return contributions.Select(contribution => new Contribution
            {
                CycleOfDelay = Convert.ToInt32(contribution.CycleOfDelay), 
                AmountTotal = Convert.ToInt32(contribution.AmountTotal)
            }).ToList();
        }

        public virtual IPagedList<Contribution> SearchContributionByCustomerId(int customerId, bool isActive = false,
            int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var query = from c in _contributionRepository.Table
                        orderby c.CreatedOnUtc
                        where c.CustomerId == customerId && c.Active == isActive
                        select c;

            var contribution = query.ToList();

            return new PagedList<Contribution>(contribution, pageIndex, pageSize);
        }

        public virtual IPagedList<Contribution> SearchContributionByLetterNumber(int letterNumberbool,
            bool isActive = false, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var query = from c in _contributionRepository.Table
                        orderby c.CreatedOnUtc
                        where c.LetterNumber == letterNumberbool && c.Active == isActive
                        select c;

            var contribution = query.ToList();

            return new PagedList<Contribution>(contribution, pageIndex, pageSize);
        }

        public virtual IPagedList<Contribution> SearchContibutionByCreatedOnUtc(DateTime? dateFrom = null,
            DateTime? dateTo = null, bool isActive = false,
            int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            if (dateFrom.HasValue && dateTo.HasValue)
            {
                var query = from c in _contributionRepository.Table
                            orderby c.CreatedOnUtc
                            where c.CreatedOnUtc.Date >= dateFrom.Value &&
                                  c.CreatedOnUtc.Date <= dateTo.Value &&
                                  c.Active == isActive
                            select c;
                var contribution = query.ToList();

                return new PagedList<Contribution>(contribution, pageIndex, pageSize);
            }

            return new PagedList<Contribution>(new List<Contribution>(), pageIndex, pageSize);
        }

        public virtual Contribution GetContributionById(int contributionId = 0, int customerId = 0, bool active = true)
        {
            if (contributionId == 0 && customerId == 0)
                return null;

            var key = string.Format(CONTRIBUTIONS_BY_KEY, contributionId, customerId, active);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _contributionRepository.Table
                            where c.Active == active
                            select c;

                if (contributionId != 0)
                {
                    query = query.Where(x => x.Id == contributionId);
                }
                if (customerId != 0)
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }
                return query.FirstOrDefault();
            });
        }

        public virtual IPagedList<ContributionPayment> GetAllPayments(int contributionId = 0, int customerId = 0,
            bool active = true, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var source = GetContributionById(contributionId, customerId, active);

            return new PagedList<ContributionPayment>(source.ContributionPayments.ToList(), pageIndex, pageSize);
        }

        public virtual void InsertContribution(Contribution contribution)
        {
            if (contribution == null)
                throw new ArgumentNullException("contribution");

            _contributionRepository.Insert(contribution);

            //cache
            _cacheManager.RemoveByPattern(CONTRIBUTIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(contribution);
        }

        public virtual void UpdateContribution(Contribution contribution)
        {
            if (contribution == null)
                throw new ArgumentNullException("contribution");

            _contributionRepository.Update(contribution);

            //cache
            _cacheManager.RemoveByPattern(CONTRIBUTIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(contribution);
        }

        #region Constants

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : contribution Id
        ///     {1} : customer Id
        ///     {2} : active
        /// </remarks>
        private const string CONTRIBUTIONS_BY_KEY = "ks.contributions.{0}-{1}-{2}";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string CONTRIBUTIONS_PATTERN_KEY = "ks.contributions.";

        #endregion

        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Contribution> _contributionRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion
    }
}