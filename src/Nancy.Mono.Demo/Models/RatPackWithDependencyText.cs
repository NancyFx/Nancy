using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nancy.Demo.Models
{
    public class RatPackWithDependencyText : RatPack
    {
        public string ApplicationDependencyText { get; set; }
        public string RequestDependencyText { get; set; }
    }
}