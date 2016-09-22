using System;
using System.Collections.Generic;
using Ks.Core;
using Ks.Core.Domain.Contract;

namespace Ks.Services.Contract
{
    public interface IContributionService
    {
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
    }
}