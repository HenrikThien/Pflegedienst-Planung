using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan.Classes
{
    public class Resource
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }
        public int RealListIndex { get; set; }
        public int ActivityId { get; set; }
    }
}
