using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splusreport.ViewModel
{
    public class StoreOrderView
    {
        public string StoreName { get; set; }
        public int TotalEmployee { get; set; }
        public int TotalTested { get; set; }
        public int TotalScore { get; set; }
        public int AverageScore { get; set; }
        public int Order { get; set; }

    }
}