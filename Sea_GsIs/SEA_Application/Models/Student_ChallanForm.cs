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
    
    public partial class Student_ChallanForm
    {
        public int Id { get; set; }
        public Nullable<int> ClassChallanFormId { get; set; }
        public Nullable<int> StudentId { get; set; }
        public Nullable<decimal> AmountPayable { get; set; }
        public Nullable<decimal> TutionFee { get; set; }
    
        public virtual AspNetStudent AspNetStudent { get; set; }
        public virtual Class_ChallanForm Class_ChallanForm { get; set; }
    }
}
