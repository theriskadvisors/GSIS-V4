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
    
    public partial class StudentRecurringFee
    {
        public int Id { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<double> LabCharges { get; set; }
        public Nullable<double> ComputerFee { get; set; }
        public Nullable<double> Other { get; set; }
        public Nullable<double> TutionFee { get; set; }
        public Nullable<double> TotalFee { get; set; }
    
        public virtual AspNetClass AspNetClass { get; set; }
    }
}
