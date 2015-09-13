using Newtonsoft.Json;
using PflegedientPlan.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan.Utils
{
    public class PatientUserMask
    {
        [JsonProperty("problems")]
        public ObservableCollection<Problem> Problems { get; set; }

        [JsonProperty("resources")]
        public ObservableCollection<Resource> Resources { get; set; }

        [JsonProperty("targets")]
        public ObservableCollection<Target> Targets { get; set; }

        [JsonProperty("measures")]
        public ObservableCollection<Measure> Measures { get; set; }
    }
}
