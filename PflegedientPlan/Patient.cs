using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string PatientVorname { get; set; }
        public string PatientNachname { get; set; }
        public string PatientGeburtsdatum { get; set; }
    }
}
