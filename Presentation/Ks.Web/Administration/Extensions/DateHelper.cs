using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ks.Services.Localization;

namespace Ks.Admin.Extensions
{
    public static class DateHelper
    {
        public static List<SelectListItem> GetMonthsList(this DateTime date, ILocalizationService localizationService)
        {
            var listOfMonth = (from i in Enumerable.Range(0, 12)
                               let now = DateTime.UtcNow.AddMonths(i)
                               select new SelectListItem
                               {
                                   Text = now.ToString("MMMM"),
                                   Value = now.Month.ToString()
                               }).OrderBy(x => int.Parse(x.Value)).ToList();

            listOfMonth.Insert(0, new SelectListItem { Value = "0", Text = localizationService.GetResource("Common.Month") });

            return listOfMonth;
        }

        public static List<SelectListItem> GetDaysList(this DateTime date, ILocalizationService localizationService)
        {
            var listOfYear = (from i in Enumerable.Range(1, 31)
                              let now = i
                              select new SelectListItem
                              {
                                  Text = now.ToString(),
                                  Value = now.ToString()
                              }).OrderBy(x => int.Parse(x.Value)).ToList();

            listOfYear.Insert(0, new SelectListItem { Value = "0", Text = localizationService.GetResource("Common.Day") });
            return listOfYear;
        }

        public static List<SelectListItem> GetYearsList(this DateTime date, ILocalizationService localizationService, int yearMin = 0, int yearMax = 10)
        {
            var listOfYear = (from i in Enumerable.Range(yearMin, yearMax)
                              let now = DateTime.UtcNow.AddYears(i)
                              select new SelectListItem
                              {
                                  Text = now.Year.ToString(),
                                  Value = now.Year.ToString()
                              }).OrderBy(x => int.Parse(x.Value)).ToList();

            listOfYear.Insert(0, new SelectListItem { Value = "0", Text = localizationService.GetResource("Common.Year") });
            return listOfYear;
        }

       
    }
}