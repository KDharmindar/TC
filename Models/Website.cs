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
    
    public partial class Website
    {
        public Website()
        {
            this.Users = new HashSet<User>();
        }
    
        public int WebID { get; set; }
        public string WebName { get; set; }
    
        public virtual ICollection<User> Users { get; set; }
    }
}
