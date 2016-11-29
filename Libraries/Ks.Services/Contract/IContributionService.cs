using System;
using System.Collections.Generic;
using Ks.Core;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Reports;

namespace Ks.Services.Contract
{
    public interface IContributionService
    {
        /// <summary>
        /// Deletes the contribution.
        /// </summary>
        /// <param name="contribution">The contribution.</param>
        void DeleteContribution(Contribution contribution);

        /// <summary>
        /// Gets the contribution group by delay.
        /// </summary>
        /// <returns></returns>
        List<Contribution> GetContributionGroupByDelay();

        /// <summary>
        /// Searches the contribution by customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<Contribution> SearchContributionByCustomerId(int customerId, int stateId = -1, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Searches the contribution by Authorize Discount.
        /// </summary>
        /// <param name="authorizeDiscountBool">The Authorize Discount.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<Contribution> SearchContributionByAuthorizeDiscount(int authorizeDiscountBool, int stateId = -1, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets the contributions by customer.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <returns></returns>
        List<Contribution> GetContributionsByCustomer(int customerId = 0, int stateId = -1);

        /// <summary>
        /// Gets the contribution by identifier.
        /// </summary>
        /// <param name="contributionId">The contribution identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <returns></returns>
        Contribution GetContributionById(int contributionId = 0, int customerId = 0, int stateId = -1);

        /// <summary>
        /// Inserts the contribution.
        /// </summary>
        /// <param name="contribution">The contribution.</param>
        void InsertContribution(Contribution contribution);

        /// <summary>
        /// Updates the contribution.
        /// </summary>
        /// <param name="contribution">The contribution.</param>
        void UpdateContribution(Contribution contribution);

        /// <summary>
        /// Gets all payments.
        /// </summary>
        /// <param name="contributionId">The contribution identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="number">The number.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="type">The type.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<ContributionPayment> GetAllPayments(int contributionId = 0,
            int number = 0, int stateId = -1, string accountNumber = "",
            bool? type = null, int pageIndex = 0, int pageSize = Int32.MaxValue);

        /// <summary>
        /// Gets the payment by identifier.
        /// </summary>
        /// <param name="contributionPaymentId">The contribution payment identifier.</param>
        /// <returns></returns>
        ContributionPayment GetPaymentById(int contributionPaymentId);

        /// <summary>
        /// Updates the contribution payment.
        /// </summary>
        /// <param name="contributionPayment">The contribution payment.</param>
        void UpdateContributionPayment(ContributionPayment contributionPayment);

        /// <summary>
        /// Gets the report contribution payment.
        /// </summary>
        /// <param name="contributionId">The contribution identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IList<ReportContributionPayment> GetReportContributionPayment(int contributionId, int pageIndex = 0, int pageSize = Int32.MaxValue);

        /// <summary>
        /// Determines whether [is payment valid] [the specified account number].
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <returns></returns>
        bool IsPaymentValid(string accountNumber, string transactionNumber);

    }
}