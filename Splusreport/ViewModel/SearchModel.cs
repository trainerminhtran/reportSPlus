using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Splusreport.Models;

namespace Splusreport.ViewModel
{
    public class SearchModel
    {
        public List<SelectScoreDMX_Result> Data { get;set; }

        public List<StoreOrderView> ListStoreOrder { get; set; }
    }
}