using System.Collections.Generic;
using System.Web.Mvc;

namespace Ks.Admin.Models.Batchs
{
    public class ScheduleBatchListModel
    {
        public ScheduleBatchListModel()
        {
            Sources =new List<SelectListItem>();
            Months = new List<SelectListItem>();
            Years= new List<SelectListItem>();
        }

        public int Source { get; set; }
        public List<SelectListItem> Sources { get; set; }
        public int MonthId { get; set; }
        public List<SelectListItem> Months { get; set; }
        public int YearId { get; set; }
        public List<SelectListItem> Years { get; set; }
    }
}