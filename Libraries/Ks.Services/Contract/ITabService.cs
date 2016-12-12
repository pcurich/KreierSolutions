using System;
using Ks.Core;
using Ks.Core.Domain.Contract;

namespace Ks.Services.Contract
{
    public interface ITabService
    {
        #region Tab
        void DeleteTab(Tab tab);
        void InsertTab(Tab tab);
        void UpdateTab(Tab tab);
        Tab GetTabById(int tabId);
        IPagedList<Tab> GetAllTabs(int pageIndex = 0,int pageSize = Int32.MaxValue);
        #endregion

        #region TabDetail
        IPagedList<TabDetail> GetAllValues(int tabId = 0, int pageIndex = 0,
           int pageSize = Int32.MaxValue);

        TabDetail GetTabDetailById(int tabDetailId);
        TabDetail GetValueFromActive(int year);
        void DeleteTabDetail(TabDetail tabDetail);
        void InsertTabDetail(TabDetail tabDetail);
        void UpdateTabDetail(TabDetail tabDetail);
        #endregion
    }
}