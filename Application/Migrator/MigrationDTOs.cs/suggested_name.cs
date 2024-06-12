using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Migrator.MigrationDTOs.cs
{
    internal class suggested_name
    {
        public int Id { get; set; }
        public string name {  get; set; }
        public string details { get; set; } 
        public string email { get; set; }
        public string geo_location_place { get; set; }
    }
}
