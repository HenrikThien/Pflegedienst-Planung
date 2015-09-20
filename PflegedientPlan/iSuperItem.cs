using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan
{
    public abstract class iSuperItem
    {
        public string SuperDescription { get; set; }
        public int SuperPosition { get; set; }
        public string SuperFrequency { get; set; }
    }
}
