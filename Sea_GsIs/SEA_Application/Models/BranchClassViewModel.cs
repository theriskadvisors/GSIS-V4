using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SEA_Application.Models
{
    public class BranchClassViewModel
    {
        [Required]
        public string ClassName { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public int SessionId { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Capacity { get; set; }
    }
}