using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core;
using Ks.Core.Data;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;

namespace Ks.Services.Contract
{
    public partial class ContributionService : IContributionService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : contribution Id
        /// {1} : customer Id
        /// </remarks>
        private const string CONTRIBUTIONS_ALL_KEY = "ks.contributions.{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CONTRIBUTIONS_PATTERN_KEY = "ks.contributions.";

        #endregion

        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Contribution> _contributionRepository;

        #endregion

        #region Constructor

        public ContributionService(IRepository<Customer> customerRepository, IRepository<Contribution> contributionRepository)
        {
            _customerRepository = customerRepository;
            _contributionRepository = contributionRepository;
        }

        #endregion

        public virtual IPagedList<Contribution> SearchContributionByCustomerId(int customerId, bool isActive = false, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var query = from c in _contributionRepository.Table
                        orderby c.CreatedOnUtc
                        where c.CustomerId == customerId && c.Active == isActive
                        select c;

            var contribution = query.ToList();

            return new PagedList<Contribution>(contribution, pageIndex, pageSize);
        }

        public virtual IPagedList<Contribution> SearchContributionByLetterNumber(int letterNumberbool, bool isActive = false, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var query = from c in _contributionRepository.Table
                        orderby c.CreatedOnUtc
                        where c.LetterNumber == letterNumberbool && c.Active == isActive
                        select c;

            var contribution = query.ToList();

            return new PagedList<Contribution>(contribution, pageIndex, pageSize);
        }

        public virtual IPagedList<Contribution> SearchContibutionByCreatedOnUtc(DateTime? dateFrom = null, DateTime? dateTo = null, bool isActive = false,
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
    }
}