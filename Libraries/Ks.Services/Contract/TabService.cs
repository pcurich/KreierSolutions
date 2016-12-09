using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf.fonts.cmaps;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Data;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Data;
using Ks.Services.Events;

namespace Ks.Services.Contract
{
    public class TabService : ITabService
    {
        #region Constants

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : Id

        /// </remarks>
        private const string TAB_BY_KEY = "ks.tabs.{0}";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string TAB_PATTERN_KEY = "ks.tabs.";

        #endregion

        #region Fields

        private readonly IRepository<Tab> _tabRepository;
        private readonly IRepository<TabDetail> _tabDetailRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Constructor

        public TabService(IRepository<Tab> tabRepository, IRepository<TabDetail> tabDetailRepository, ICacheManager cacheManager, IEventPublisher eventPublisher, IDataProvider dataProvider, IDbContext dbContext)
        {
            _tabRepository = tabRepository;
            _tabDetailRepository = tabDetailRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        #region Tab

        public virtual void DeleteTab(Tab tab)
        {
            if (tab == null)
                throw new ArgumentNullException("tab");

            _tabRepository.Delete(tab);

            //cache
            _cacheManager.RemoveByPattern(TAB_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(tab);
        }

        public virtual void InsertTab(Tab tab)
        {
            if (tab == null)
                throw new ArgumentNullException("tab");

            _tabRepository.Insert(tab);

            //cache
            _cacheManager.RemoveByPattern(TAB_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(tab);
        }

        public virtual void UpdateTab(Tab tab)
        {
            if (tab == null)
                throw new ArgumentNullException("tab");

            _tabRepository.Update(tab);

            //cache
            _cacheManager.RemoveByPattern(TAB_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(tab);
        }

        public virtual Tab GetTabById(int tabId)
        {
            var query = from c in _tabRepository.Table
                        where c.Id == tabId
                        select c;

            var result = query.FirstOrDefault();
            return result;
        }

        public virtual IPagedList<Tab> GetAllTabs(bool active = true, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from c in _tabRepository.Table
                        select c;
            return new PagedList<Tab>(query.ToList(), pageIndex, pageSize);
        }

        #endregion

        #region TabDetails

        public virtual IPagedList<TabDetail> GetAllValues(int tabId = 0, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            if (tabId == 0)
                return new PagedList<TabDetail>(new List<TabDetail>(), pageIndex, pageSize);

            var query = from t in _tabRepository.Table
                        join td in _tabDetailRepository.Table on t.Id equals td.TabId
                        where t.Id == tabId
                        select td;

            return new PagedList<TabDetail>(query.ToList(), pageIndex, pageSize);

        }

        public virtual void DeleteTabDetail(TabDetail tabDetail)
        {
            if (tabDetail == null)
                throw new ArgumentNullException("tabDetail");

            _tabDetailRepository.Delete(tabDetail);

            //cache
            _cacheManager.RemoveByPattern(TAB_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(tabDetail);
        }

        public virtual void InsertTabDetail(TabDetail tabDetail)
        {
            if (tabDetail == null)
                throw new ArgumentNullException("tabDetail");

            _tabDetailRepository.Insert(tabDetail);

            //cache
            _cacheManager.RemoveByPattern(TAB_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(tabDetail);
        }

        public virtual void UpdateTabDetail(TabDetail tabDetail)
        {
            if (tabDetail == null)
                throw new ArgumentNullException("tabDetail");

            _tabDetailRepository.Update(tabDetail);

            //cache
            _cacheManager.RemoveByPattern(TAB_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(tabDetail);
        }

        #endregion

        #endregion
    }
}