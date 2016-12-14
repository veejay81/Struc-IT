using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StructureITWebAPIFrontEnd.Models
{
    public class StructureITDBContext : DbContext
    {
        public StructureITDBContext() : base("name=StructureITDBContext")
        {
        }

        public System.Data.Entity.DbSet<StructureITWebAPIFrontEnd.Models.Artists> Artists { get; set; }
    }
}