using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CSCTask2v2.Models
{
    public class Context:DbContext
    {
        public Context() : base("StringDBContext") { }
        public DbSet<ProductModel> Product { get; set; }
    }
}