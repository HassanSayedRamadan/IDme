using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDme.Models
{
    internal class Status
    {
        public string group { get; set; }
        public List<string> subgroups { get; set; }
        public bool? verified { get; set; }
    }
}
