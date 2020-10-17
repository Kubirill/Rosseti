using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosseti
{
    
    public class Employer
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string role { get; set; }
        public string safety_group { get; set; }
        public int id { get; set; }
    }

    public class Places
    {
        public int id { get; set; }
        public string name { get; set; }

        public Location location { get; set; }
    }
    public class Location
    {
        public double lat { get; set; }
        
        public double lng { get; set; }
        
    }
    public class Task
    {
        public Employer creator{ get; set; }
        public Employer executor { get; set; }
        public int id { get; set; }

        public Places place { get; set; }

        public string safety_event { get; set; }
    }
}
