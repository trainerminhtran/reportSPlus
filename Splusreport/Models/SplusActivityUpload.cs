//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Splusreport.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SplusActivityUpload
    {
        public int ID { get; set; }
        public string LoginID { get; set; }
        public string MNV { get; set; }
        public string Jobgroup { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityName { get; set; }
        public System.DateTime AttempStartdate { get; set; }
        public System.DateTime AttempEnddate { get; set; }
        public int Score { get; set; }
        public decimal SecondTest { get; set; }
        public decimal SecondLearn { get; set; }
        public int TimesLearn { get; set; }
        public string IsLearned { get; set; }
        public string IsTested { get; set; }
        public string IsComplete { get; set; }
    

    }
}
