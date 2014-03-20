using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EaWebApi.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private List<Package> packages = new List<Package>();
        public List<Package> Packages
        {
            get { return packages; } 
        }

        private List<Diagram> diagrams = new List<Diagram>();
        public List<Diagram> Diagrams
        {
            get { return diagrams; }
        }
    }
}