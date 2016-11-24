using System;
using System.Collections.Generic;
using Ks.Core;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Reports;

namespace Ks.Services.Contract
{
    public interface ILoanService
    {
        /// <summary>
        /// Deletes the loan.
        /// </summary>
        /// <param name="loan">The loan.</param>
        void DeleteLoan(Loan loan);
        /// <summary>
        /// Searches the loan by customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<Loan> SearchLoanByCustomerId(int customerId, int stateId = -1, int pageIndex = 0, int pageSize = int.MaxValue);
        /// <summary>
        /// Searches the loan by loan number.
        /// </summary>
        /// <param name="loanNumber">The loan number.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<Loan> SearchLoanByLoanNumber(int loanNumber, int stateId = -1, int pageIndex = 0, int pageSize = int.MaxValue);
        /// <summary>
        /// Gets the loan by identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="onlyActive"></param>
        /// <returns></returns>
        List<Loan> GetLoansByCustomer(int customerId = 0, bool onlyActive=true);
        /// <summary>
        /// Gets the loan by identifier.
        /// </summary>
        /// <param name="loanId">The loan identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <returns></returns>
        Loan GetLoanById(int loanId = 0, int customerId = 0, int stateId = -1);
        /// <summary>
        /// Inserts the loan.
        /// </summary>
        /// <param name="loan">The loan.</param>
        void InsertLoan(Loan loan);
        /// <summary>
        /// Updates the loan.
        /// </summary>
        /// <param name="loan">The loan.</param>
        void UpdateLoan(Loan loan);
        /// <summary>
        /// Gets all payments.
        /// </summary>
        /// <param name="loanId">The loan identifier.</param>
        /// <param name="quota">The quota.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="type">The type.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<LoanPayment> GetAllPayments(int loanId = 0, int quota = 0,
            int stateId = -1, string accountNumber = "0", bool? type = null,
            bool active = true, int pageIndex = 0,
            int pageSize = Int32.MaxValue);

        /// <summary>
        /// Gets the payment by identifier.
        /// </summary>
        /// <param name="loanPaymentId">The loan payment identifier.</param>
        /// <returns></returns>
        LoanPayment GetPaymentById(int loanPaymentId);
        /// <summary>
        /// Updates the loan payment.
        /// </summary>
        /// <param name="loanPayment">The loan payment.</param>
        void UpdateLoanPayment(LoanPayment loanPayment);

        IList<ReportLoanPayment> GetReportLoanPayment(int loanId, int pageIndex = 0, int pageSize = Int32.MaxValue);
    }
}