using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SEA_Application.Models
{
    public class StudentUserViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Roll Number")]
        public string RollNo { get; set; }
        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "User name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} can't be empty")]
        [RegularExpression("^[a-zA-Z0-9]([._](?![._])|[a-zA-Z0-9]){6,18}[a-zA-Z0-9]$", ErrorMessage = "{0} must contain atleast 6 digits and no spaces")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int Class { get; set; }
        [Required]
        public int Section { get; set; }
        [Required]
        public int Session { get; set; }
    }
}