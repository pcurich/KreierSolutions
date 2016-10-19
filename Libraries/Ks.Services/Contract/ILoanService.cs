using System;
using Ks.Core;
using Ks.Core.Domain.Contract;

namespace Ks.Services.Contract
{
    public interface ILoanService
    {
        void DeleteLoan(Loan loan);
        IPagedList<Loan> SearchLoanByCustomerId(int customerId, bool isActive = false, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Loan> SearchLoanByLoanNumber(Guid loanNumber, bool isActive = false, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Loan> SearchLoanByCreatedOnUtc(DateTime? dateFrom = null, DateTime? dateTo = null, bool isActive = false, int pageIndex = 0, int pageSize = int.MaxValue);
        Loan GetLoanById(int loanId = 0, int customerId = 0, bool active = true);
        IPagedList<LoanPayment> GetAllPayments(int loanId = 0, int customerId = 0, bool active = true,
            int pageIndex = 0, int pageSize = int.MaxValue);
        void InsertLoan(Loan loan);
        void UpdateLoan(Loan loan);
    }
}