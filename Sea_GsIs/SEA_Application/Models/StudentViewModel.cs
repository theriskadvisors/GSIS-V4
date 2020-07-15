using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SEA_Application.Models
{
    public class StudentViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string RollNo { get; set; }
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
        public string UserId { get; set; }
        [Required]
        public int BranchId { get; set; }
    }
}