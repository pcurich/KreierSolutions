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
        /// Gets the contribution by customer identifier.
        /// Only with Dni or AdmCode
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        IPagedList<Contribution> SearchContributionByCustomerId(int customerId, bool isActive = false, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets the contribution by letter number.
        /// </summary>
        /// <param name="letterNumberbool">The letter number. If is zero, then select all</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        IPagedList<Contribution> SearchContributionByLetterNumber(int letterNumberbool, bool isActive = false, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets the contibution by a range of dates on UTC.
        /// </summary>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="dateTo">The date to.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        IPagedList<Contribution> SearchContibutionByCreatedOnUtc(DateTime? dateFrom = null, DateTime? dateTo = null, bool isActive = false, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets the contribution by identifier.
        /// </summary>
        /// <param name="contributionId">The contribution identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <returns></returns>
        Contribution GetContributionById(int contributionId=0, int customerId=0, bool active = true);

        /// <summary>
        /// Gets all payments.
        /// </summary>
        /// <param name="contributionId">The contribution identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<ContributionPayment> GetAllPayments(int contributionId = 0, int customerId = 0, bool active = true,
            int pageIndex = 0, int pageSize = int.MaxValue);

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
    }
}