﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDme.Models
{
    internal class tokenBody
    {
        public string code { get; set; }
        public string code_verifier { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string redirect_uri { get; set; }
        public string grant_type { get; set; }
    }
}
