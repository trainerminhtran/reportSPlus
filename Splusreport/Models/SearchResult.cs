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
        public int SecondTest { get; set; }

        public int Score { get; set; }
        public int SecondLearn { get; set; }
        public int TimesLearn { get; set; }
    }

    public class NoChange
    {
        public LearnTest DMX { get; set; }
        public LearnTest MM { get; set; }
        public LearnTest PICO { get; set; }
        public LearnTest VHC { get; set; }
        public LearnTest NK { get; set; }
        public LearnTest CP { get; set; }
        public LearnTest OTher { get; set; }
    }
    public class LearnTest
    {
        public int Learn { get; set; }
        public int Test { get; set; }
    }
}