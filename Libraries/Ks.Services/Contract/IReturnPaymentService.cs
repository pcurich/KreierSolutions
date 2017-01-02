using System;
using Ks.Core;
using Ks.Core.Domain.Contract;

namespace Ks.Services.Contract
{
    public interface IReturnPaymentService
    {
        void InsertReturnPayment(ReturnPayment returnPayment);
        void UpdateReturnPayment(ReturnPayment returnPayment);

        ReturnPayment GetReturnPaymentById(int returnPaymentId);

        IPagedList<ReturnPayment> SearchReturnPayment(int customerId, int searchTypeId,
            int paymentNumber, int pageIndex = 0, int pageSize = Int32.MaxValue);
    }
}