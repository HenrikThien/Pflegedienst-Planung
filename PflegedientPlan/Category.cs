using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan
{
    public class Category
    {
        public int Id { get; set; }
        public int ParentId { get; set; }

        public string ActivityDescription { get; set; }
        public string CategoryDescription { get; set; }

        public override string ToString()
        {
            return CategoryDescription;
        }
    }
}
