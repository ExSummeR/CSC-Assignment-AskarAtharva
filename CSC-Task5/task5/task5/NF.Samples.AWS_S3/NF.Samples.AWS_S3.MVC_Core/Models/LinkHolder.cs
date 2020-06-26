using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NF.Samples.AWS_S3.MVC_Core.Models
{
    public class LinkHolder 
    {
       public string link { get; set; }
        public string BitlyLink { get; set; }
    }
}
