using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splusreport.ViewModel
{
    public class SplusActivityUpload
    {
        public string LoginID { get; set; }
        public string JobGroup { get; set; }
        public string ActivityCode { get; set; }
        public DateTime AttempEndDate { get; set; }
        public int Score { get; set; }
    }
}