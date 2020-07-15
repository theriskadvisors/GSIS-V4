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
    
    public partial class StudentFeeMultiplier
    {
        public int Id { get; set; }
        public Nullable<int> StudentId { get; set; }
        public Nullable<double> TutionFee { get; set; }
        public Nullable<double> PaidAmount { get; set; }
        public Nullable<double> RemainingAmount { get; set; }
        public Nullable<double> SharePerInstalment { get; set; }
        public Nullable<int> Instalments { get; set; }
        public Nullable<double> PaidInstalments { get; set; }
        public Nullable<double> RemainingInstalments { get; set; }
        public Nullable<double> Multiplier { get; set; }
        public Nullable<double> TotalPayableFee { get; set; }
    
        public virtual AspNetStudent AspNetStudent { get; set; }
    }
}
