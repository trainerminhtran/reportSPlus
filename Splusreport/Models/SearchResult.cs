using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splusreport.Models
{
    public class SearchResult
    {
        public string SPlusCode { get; set; }
        public string Fullname { get; set; }
        public string Store { get; set; }
        public string Region { get; set; }
        public string ActivityCode { get; set; }
        public string IsLearned { get; set; }
        public int Score { get; set; }
    }
}