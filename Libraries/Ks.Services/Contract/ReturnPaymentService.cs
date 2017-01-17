using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Services.Events;

namespace Ks.Services.Contract
{
    public class ReturnPaymentService : IReturnPaymentService
    {
        #region Contructor

        public ReturnPaymentService(IRepository<ReturnPayment> returnPaymentRepository,
               ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            _returnPaymentRepository = returnPaymentRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

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

        public virtual IPagedList<ReturnPayment> SearchReturnPayment(int customerId,
            int searchTypeId, int paymentNumber, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            if (customerId == 0 && (searchTypeId == 0 || paymentNumber == 0))
                return new PagedList<ReturnPayment>(new List<ReturnPayment>(), pageIndex, pageSize);

            var query = from rp in _returnPaymentRepository.Table
                        select rp;

            if (customerId > 0)
                query = query.Where(x => x.CustomerId == customerId);
            else
            {
                if (searchTypeId != 0 && paymentNumber != 0)
                {
                    query = query.Where(x => x.StateId == searchTypeId);
                    query = query.Where(x => x.PaymentNumber == paymentNumber);
                }
            }
            return new PagedList<ReturnPayment>(query.ToList(), pageIndex, pageSize);

        }

        #endregion
    }
}