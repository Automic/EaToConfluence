using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EaWebApi.Models
{
    public class Diagram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private List<Element> elements = new List<Element>();
        public IList<Element> Elements
        {
            get { return elements; }
        }
    }
}