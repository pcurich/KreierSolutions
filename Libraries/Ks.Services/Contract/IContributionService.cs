using System;
using System.Collections.Generic;
using Ks.Core;
using Ks.Core.Domain.Contract;

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
        /// Searches the contribution by letter number.
        /// </summary>
        /// <param name="letterNumberbool">The letter numberbool.</param>
        /// <param name="stateId">The state identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<Contribution> SearchContributionByLetterNumber(int letterNumberbool, int stateId = -1, int pageIndex = 0, int pageSize = int.MaxValue);

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
        Contribution GetContributionById(int contributionId=0, int customerId=0, int stateId = -1);

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
        /// <param name="stateId">The state identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<ContributionPayment> GetAllPayments(int contributionId = 0, int customerId = 0, int stateId = -1,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets the payment by identifier.
        /// </summary>
        /// <param name="contributionPaymentId">The contribution payment identifier.</param>
        /// <returns></returns>
        ContributionPayment GetPaymentById(int contributionPaymentId);

        void UpdateContributionPayment(ContributionPayment contributionPayment);
    }
}