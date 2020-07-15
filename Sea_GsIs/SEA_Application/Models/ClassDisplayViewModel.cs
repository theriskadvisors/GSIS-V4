using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEA_Application.Models
{
    public class ClassDisplayViewModel
    {
        public AspNetClass Class { get; set; }
        public AspNetBranch Branch { get; set; }
        public AspNetSession Session { get; set; }
        public int Capacity { get; set; }
    }
}