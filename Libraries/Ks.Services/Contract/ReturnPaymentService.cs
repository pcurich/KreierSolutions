using System;
using System.Linq;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Contract;
using Ks.Services.Events;

namespace Ks.Services.Contract
{
    public class ReturnPaymentService : IReturnPaymentService
    {
        #region Constants

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : customer Id
        /// </remarks>
        private const string RETURN_BY_KEY = "k.return.{0}";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string RETURN_PATTERN_KEY = "ks.return.";

        #endregion

        #region Contructor

        public ReturnPaymentService(IRepository<ReturnPayment> returnPaymentRepository, ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            _returnPaymentRepository = returnPaymentRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Fieds

        private readonly IRepository<ReturnPayment> _returnPaymentRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Method

        public virtual void InsertReturnPayment(ReturnPayment returnPayment)
        {
            if (returnPayment == null)
                throw new ArgumentNullException("returnPayment");

            _returnPaymentRepository.Insert(returnPayment);

            //cache
            _cacheManager.RemoveByPattern(RETURN_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(returnPayment);
        }

        public virtual void UpdateReturnPayment(ReturnPayment returnPayment)
        {
            if (returnPayment == null)
                throw new ArgumentNullException("returnPayment");

            _returnPaymentRepository.Update(returnPayment);

            //cache
            _cacheManager.RemoveByPattern(RETURN_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(returnPayment);
        }

        public virtual ReturnPayment GetReturnPaymentById(int returnPaymentId)
        {
            var query = from r in _returnPaymentRepository.Table
                        where r.Id == returnPaymentId
                        select r;

            return query.FirstOrDefault();
        }

        public virtual IPagedList<ReturnPayment> GetReturnPayments(int customerId, int stateId = -1, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var key = string.Format(RETURN_BY_KEY, customerId);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _returnPaymentRepository.Table
                            select c;
                if (stateId >= 0)
                {
                    query = query.Where(x => x.StateId == stateId);
                }

                if (customerId != 0)
                    query = query.Where(x => x.CustomerId == customerId);

                return new PagedList<ReturnPayment>(query.ToList(), pageIndex, pageSize);
            });
        }

        #endregion
    }
}