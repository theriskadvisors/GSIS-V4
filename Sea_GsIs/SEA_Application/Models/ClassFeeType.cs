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
    
    public partial class ClassFeeType
    {
        public int Id { get; set; }
        public Nullable<int> ClassFeeId { get; set; }
        public Nullable<int> ClassId { get; set; }
    
        public virtual AspNetClass AspNetClass { get; set; }
        public virtual ClassFee ClassFee { get; set; }
    }
}
