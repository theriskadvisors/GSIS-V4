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
    
    public partial class AspNetParent
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public string FatherName { get; set; }
        public string FatherCellNo { get; set; }
        public string FatherEmail { get; set; }
        public string FatherOccupation { get; set; }
        public string FatherEmployer { get; set; }
        public string MotherName { get; set; }
        public string MotherCellNo { get; set; }
        public string MotherEmail { get; set; }
        public string MotherOccupation { get; set; }
        public string MotherEmployer { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
