using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan
{
    public enum Gender
    {
        MALE = 1,
        FEMALE = 2
    }

    public class Patient
    {
        public int PatientId { get; set; }
        public string PatientVorname { get; set; }
        public string PatientNachname { get; set; }
        public string PatientGeburtsdatum { get; set; }
        public Gender PatientGender { get; set; }

        public string PatientGenderToString
        {
            get
            {
                return (PatientGender == Gender.MALE) ? "Männlich" : "Weiblich";
            }
        }
    }
}
