using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Services.Events;

namespace Ks.Services.Contract
{
    public class ContributionService : IContributionService
    {
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
        private readonly IRepository<ContributionPayment> _contributionPaymentRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Constructor

        public ContributionService(IRepository<Customer> customerRepository,
            IRepository<Contribution> contributionRepository,
            IRepository<ContributionPayment> contributionPaymentRepository, 
            ICacheManager cacheManager, 
            IEventPublisher eventPublisher)
        {
            _customerRepository = customerRepository;
            _contributionRepository = contributionRepository;
            _contributionPaymentRepository = contributionPaymentRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods
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
 

        public virtual IPagedList<Contribution> SearchContributionByCustomerId(int customerId, int stateId = -1,
            int pageIndex = 0,int pageSize = Int32.MaxValue)
        {
            var query = from c in _contributionRepository.Table
                        orderby c.CreatedOnUtc
                        where c.CustomerId == customerId 
                        select c;
            if (stateId >= 0)
            {
                query = stateId == 0 ? query.Where(x => x.Active == false) : query.Where(x => x.Active == true);
            }
            var contribution = query.ToList();

            return new PagedList<Contribution>(contribution, pageIndex, pageSize);
        }

        public virtual IPagedList<Contribution> SearchContributionByLetterNumber(int letterNumberbool,
            int stateId = -1, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            
            var query = from c in _contributionRepository.Table
                        orderby c.CreatedOnUtc
                        where c.AuthorizeDiscount == letterNumberbool
                        select c;

            if (stateId >= 0)
            {
                query = stateId == 0 ? query.Where(x => x.Active == false) : query.Where(x => x.Active == true);
            }

            var contribution = query.ToList();

            return new PagedList<Contribution>(contribution, pageIndex, pageSize);
        }

        public virtual List<Contribution> GetContributionsByCustomer(int customerId = 0, int stateId = -1)
        {
            if (customerId == 0)
                return new List<Contribution>();

            var key = string.Format(CONTRIBUTIONS_BY_KEY, int.MaxValue, customerId, stateId);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _contributionRepository.Table
                            select c;
                if (stateId >= 0)
                {
                    query = stateId == 0 ? query.Where(x => x.Active == false) : query.Where(x => x.Active == true);
                }
                if (customerId != 0)
                    query = query.Where(x => x.CustomerId == customerId);

                query = query.OrderByDescending(x => x.CreatedOnUtc);
                return query.ToList();
                 
            });
        }

        public virtual Contribution GetContributionById(int contributionId = 0, int customerId = 0, int stateId = -1)
        {
            if (customerId == 0 && contributionId==0)
                return null;

            var key = string.Format(CONTRIBUTIONS_BY_KEY, contributionId, customerId, stateId);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _contributionRepository.Table
                            select c;
                if (stateId >= 0)
                {
                    query = stateId==0 ? query.Where(x => x.Active == false) : query.Where(x => x.Active == true);
                }
                if (contributionId != 0)
                    query = query.Where(x => x.Id == contributionId);
                
                if (customerId != 0)
                    query = query.Where(x => x.CustomerId == customerId);
                return query.FirstOrDefault();
            });
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

        public virtual IPagedList<ContributionPayment> GetAllPayments(int contributionId = 0, int customerId = 0,
            int stateId = -1, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var source = GetContributionById(contributionId, customerId, stateId);
            return source != null ?
                new PagedList<ContributionPayment>(source.ContributionPayments.ToList(), pageIndex, pageSize) :
                new PagedList<ContributionPayment>(new List<ContributionPayment>(), pageIndex, pageSize);
        }

        public virtual ContributionPayment GetPaymentById(int contributionPaymentId)
        {
            var query = from c in _contributionPaymentRepository.Table
                        where c.Id == contributionPaymentId
                        select c;

            var result = query.FirstOrDefault();
            return result;
        }

        public virtual void UpdateContributionPayment(ContributionPayment contributionPayment)
        {
            if (contributionPayment == null)
                throw new ArgumentNullException("contributionPayment");

            _contributionPaymentRepository.Update(contributionPayment);

            //cache
            _cacheManager.RemoveByPattern(CONTRIBUTIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(contributionPayment);
        }
        #endregion
    }
}