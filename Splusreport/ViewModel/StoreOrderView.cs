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
        public int TotalSecond { get; set; }
        public decimal AverageScore { get; set; }
        public decimal AverageSecond { get; set; }
        public decimal RateTested { get; set; }

        public decimal Gold { get; set; }
        public decimal SecondTest { get; set; }
        public int Order { get; set; }

    }
}