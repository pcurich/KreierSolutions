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
        ///     {0} :  Id
        ///     {1} : customer Id
        /// </remarks>
        private const string LOANS_BY_KEY = "ks.loans.{0}-{1}";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string LOANS_PATTERN_KEY = "ks.loans.";

        #endregion

        #region Fields

        private readonly IRepository<LoanPayment> _loanPaymentRepository;
        private readonly IRepository<Loan> _loanRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Constructor

        public LoanService(IRepository<LoanPayment> loanPaymentRepository,
            IRepository<Loan> loanRepository, ICacheManager cacheManager, IEventPublisher eventPublisher)
        {
            _loanPaymentRepository = loanPaymentRepository;
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

        public virtual IPagedList<Loan> SearchLoanByCustomerId(int customerId, int stateId = -1, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from c in _loanRepository.Table
                        orderby c.CreatedOnUtc
                        where c.CustomerId == customerId
                        select c;
            if (stateId >= 0)
            {
                query = stateId == 0 ? query.Where(x => x.Active == false) : query.Where(x => x.Active == true);
            }

            var loans = query.ToList();

            return new PagedList<Loan>(loans, pageIndex, pageSize);
        }

        public virtual IPagedList<Loan> SearchLoanByLoanNumber(int loanNumber, int stateId = -1, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from c in _loanRepository.Table
                        orderby c.CreatedOnUtc
                        where c.LoanNumber == loanNumber
                        select c;

            if (stateId >= 0)
            {
                query = stateId == 0 ? query.Where(x => x.Active == false) : query.Where(x => x.Active == true);
            }

            var loans = query.ToList();

            return new PagedList<Loan>(loans, pageIndex, pageSize);
        }
        
        public virtual List<Loan> GetLoansByCustomer(int customerId = 0,bool onlyActive=true)
        {
            if (customerId == 0)
                return null;

            var key = string.Format(LOANS_BY_KEY, 0,customerId);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _loanRepository.Table
                            where c.CustomerId == customerId
                            select c;

                if (onlyActive)
                    query = query.Where(x => x.Active);

                return query.ToList();
            });
        }

        public virtual Loan GetLoanById(int loanId = 0, int customerId = 0, int stateId = -1)
        {
            if (customerId == 0 && loanId == 0)
                return null;

            var key = string.Format(LOANS_BY_KEY, loanId, customerId);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _loanRepository.Table
                            select c;
                if (stateId >= 0)
                {
                    query = stateId == 0 ? query.Where(x => x.Active == false) : query.Where(x => x.Active == true);
                }
                if (loanId != 0)
                    query = query.Where(x => x.Id == loanId);

                if (customerId != 0)
                    query = query.Where(x => x.CustomerId == customerId);

                return query.FirstOrDefault();
            });
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

        public virtual IPagedList<LoanPayment> GetAllPayments(int loanId = 0, int quota = 0,
            int stateId = -1, string accountNumber = "0", bool? type = null,
            bool active = true, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var query = from c in _loanPaymentRepository.Table
                        select c;

            if (loanId != 0)
                query = query.Where(x => x.LoanId == loanId);
            if (quota != 0)
                query = query.Where(x => x.Quota == quota);
            if (stateId != 0)
                query = query.Where(x => x.StateId == stateId);
            if (!"0".Equals(accountNumber))
                query = query.Where(x => x.AccountNumber == accountNumber);
            if (type != null)
                query = query.Where(x => x.IsAutomatic == type.Value);


            return new PagedList<LoanPayment>(query.ToList(), pageIndex, pageSize);
        }

        public virtual LoanPayment GetPaymentById(int loanPaymentId)
        {
            var query = from c in _loanPaymentRepository.Table
                        where c.Id == loanPaymentId
                        select c;

            var result = query.FirstOrDefault();
            return result;
        }

        public virtual void UpdateLoanPayment(LoanPayment loanPayment)
        {
            if (loanPayment == null)
                throw new ArgumentNullException("loanPayment");

            _loanPaymentRepository.Update(loanPayment);

            //cache
            _cacheManager.RemoveByPattern(LOANS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(loanPayment);
        }

        #endregion
    }
}