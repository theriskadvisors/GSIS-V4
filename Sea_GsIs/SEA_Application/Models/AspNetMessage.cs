//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SEA_Application.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetMessage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetMessage()
        {
            this.AspNetMessage_Receiver = new HashSet<AspNetMessage_Receiver>();
        }
    
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public string SenderID { get; set; }
        public Nullable<bool> IsEmail { get; set; }
        public Nullable<bool> IsText { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetMessage_Receiver> AspNetMessage_Receiver { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
