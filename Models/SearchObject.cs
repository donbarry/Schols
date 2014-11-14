using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Schols.Models
{
    public class SearchObject
    {
        public String title {get; set;}
        public String purpose { get; set; }
        public String department { get; set; }
        public String college { get; set; }
        public String schoolYear { get; set; }
        public String major { get; set; }
        public String undergradGPA { get; set; }
        public String gradGPA { get; set; }
        public String highschoolGPA { get; set; }
    }
}