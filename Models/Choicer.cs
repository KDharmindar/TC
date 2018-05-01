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
    
    public partial class Choicer
    {
        public Choicer()
        {
            this.ChoiceShares = new HashSet<ChoiceShare>();
            this.ChoiceTests = new HashSet<ChoiceTest>();
        }
    
        public int ChoicerID { get; set; }
        public Nullable<int> FCategoryID { get; set; }
        public Nullable<int> FCUserID { get; set; }
        public string CQuestion { get; set; }
        public string Opt1 { get; set; }
        public string Opt2 { get; set; }
        public string Opt3 { get; set; }
        public string Opt4 { get; set; }
        public string Opt5 { get; set; }
        public Nullable<int> CAnswer { get; set; }
        public Nullable<bool> IsShareOnline { get; set; }
        public Nullable<bool> IsFavourite { get; set; }
        public Nullable<bool> IsSkip { get; set; }
        public Nullable<int> Appearance { get; set; }
        public Nullable<int> WAppearance { get; set; }
        public Nullable<int> Score { get; set; }
        public string AdditionalInfo { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ChoiceShare> ChoiceShares { get; set; }
        public virtual ICollection<ChoiceTest> ChoiceTests { get; set; }
    }
}
