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
    public class SearchModelMM
    {
        public List<SelectScoreMM_Result> Data { get; set; }

        public List<StoreOrderView> ListStoreOrder { get; set; }
    } public class SearchModelNK
    {
        public List<SelectScoreNK_Result> Data { get; set; }

        public List<StoreOrderView> ListStoreOrder { get; set; }
    } public class SearchModelVHC
    {
        public List<SelectScoreVHC_Result> Data { get; set; }

        public List<StoreOrderView> ListStoreOrder { get; set; }
    }public class SearchModelPICO
    {
        public List<SelectScorePICO_Result> Data { get; set; }

        public List<StoreOrderView> ListStoreOrder { get; set; }
    }
}