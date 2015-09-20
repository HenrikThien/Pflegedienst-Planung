using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan.Classes
{
    public class Resource : iSuperItem
    {
        public int Id { get; set; }
        public int Position
        {
            get { return base.SuperPosition; }
            set { base.SuperPosition = value; }
        }
        public string Description
        {
            get { return base.SuperDescription; }
            set { base.SuperDescription = value; }
        }
        public bool IsChecked { get; set; }
        public int RealListIndex { get; set; }
        public int ActivityId { get; set; }
    }
}
