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
    
    public partial class UserAutoAbsent
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
