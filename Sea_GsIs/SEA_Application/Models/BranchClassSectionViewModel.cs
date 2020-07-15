using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SEA_Application.Models
{
    public class BranchClassSectionViewModel
    {
        [Required]
        public string SectionName { get; set; }
        [Required]
        public int BranchClassId { get; set; }
    }
}