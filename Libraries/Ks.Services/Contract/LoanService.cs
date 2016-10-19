using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Services.Events;

namespace Ks.Services.Contract
{
    public partial class LoanService : ILoanService
    {
        #region Constants

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : loan Id
        ///     {1} : customer Id
        ///     {2} : active
        /// </remarks>
        private const string LOANS_BY_KEY = "ks.loans.{0}-{1}-{2}";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string LOANS_PATTERN_KEY = "ks.loans.";

        #endregion

        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Loan> _loanRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Constructor

        public LoanService(IRepository<Customer> customerRepository,
            IRepository<Loan> loanRepository, ICacheManager cacheManager, IEventPublisher eventPublisher)
        {
            _customerRepository = customerRepository;
            _loanRepository = loanRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public virtual void DeleteLoan(Loan loan)
        {
            if (loan == null)
                throw new ArgumentNullException("loan");

            loan.Active = false;
            UpdateLoan(loan);
        }

        public virtual IPagedList<Loan> SearchLoanByCustomerId(int customerId, bool isActive = false, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from c in _loanRepository.Table
                        orderby c.CreatedOnUtc
                        where c.CustomerId == customerId && c.Active == isActive
                        select c;

            var contribution = query.ToList();

            return new PagedList<Loan>(contribution, pageIndex, pageSize);
        }

        public virtual IPagedList<Loan> SearchLoanByLoanNumber(Guid loanNumber, bool isActive = false, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from c in _loanRepository.Table
                        orderby c.CreatedOnUtc
                        where c.LoanNumber == loanNumber && c.Active == isActive
                        select c;

            var contribution = query.ToList();

            return new PagedList<Loan>(contribution, pageIndex, pageSize);
        }

        public virtual IPagedList<Loan> SearchLoanByCreatedOnUtc(DateTime? dateFrom = null, DateTime? dateTo = null, bool isActive = false,
            int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            if (dateFrom.HasValue && dateTo.HasValue)
            {
                var query = from c in _loanRepository.Table
                            orderby c.CreatedOnUtc
                            where c.CreatedOnUtc.Date >= dateFrom.Value &&
                                  c.CreatedOnUtc.Date <= dateTo.Value &&
                                  c.Active == isActive
                            select c;
                var contribution = query.ToList();

                return new PagedList<Loan>(contribution, pageIndex, pageSize);
            }

            return new PagedList<Loan>(new List<Loan>(), pageIndex, pageSize);
        }

        public virtual Loan GetLoanById(int loanId = 0, int customerId = 0, bool active = true)
        {
            if (loanId == 0 && customerId == 0)
                return null;

            var key = string.Format(LOANS_BY_KEY, loanId, customerId, active);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _loanRepository.Table
                            where c.Active == active
                            select c;

                if (loanId != 0)
                {
                    query = query.Where(x => x.Id == loanId);
                }
                if (customerId != 0)
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }
                return query.FirstOrDefault();
            });
        }

        public virtual IPagedList<LoanPayment> GetAllPayments(int loanId = 0, int customerId = 0, bool active = true, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var source = GetLoanById(loanId, customerId, active);

            return new PagedList<LoanPayment>(source.LoanPayments.ToList(), pageIndex, pageSize);
        }

        public virtual void InsertLoan(Loan loan)
        {
            if (loan == null)
                throw new ArgumentNullException("loan");

            _loanRepository.Insert(loan);

            //cache
            _cacheManager.RemoveByPattern(LOANS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(loan);
        }

        public virtual void UpdateLoan(Loan loan)
        {
            if (loan == null)
                throw new ArgumentNullException("loan");

            _loanRepository.Update(loan);

            //cache
            _cacheManager.RemoveByPattern(LOANS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(loan);
        }

        #endregion
    }
}