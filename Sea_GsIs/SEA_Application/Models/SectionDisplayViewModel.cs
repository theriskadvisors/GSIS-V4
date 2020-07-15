using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SEA_Application.Models
{
    public class SectionDisplayViewModel
    {
        public AspNetSection Section { get; set; }
        public AspNetClass Class { get; set; }
        public AspNetBranch Branch { get; set; }
        public AspNetSession Session { get; set; }
    }
}