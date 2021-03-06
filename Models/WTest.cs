//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace tutioncloud.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class WTest
    {
        public WTest()
        {
            this.TestShares = new HashSet<TestShare>();
            this.WordTests = new HashSet<WordTest>();
        }
    
        public int WTestID { get; set; }
        public Nullable<int> FWTUserID { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> Score { get; set; }
        public string TNote { get; set; }
        public string Status { get; set; }
        public Nullable<bool> TestTime { get; set; }
        public Nullable<bool> TimePerWord { get; set; }
        public Nullable<System.DateTime> Minutes { get; set; }
        public Nullable<System.DateTime> Seconds { get; set; }
        public Nullable<System.DateTime> Duration { get; set; }
    
        public virtual ICollection<TestShare> TestShares { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<WordTest> WordTests { get; set; }
    }
}
