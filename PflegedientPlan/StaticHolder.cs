using PflegedientPlan.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan
{
    static class StaticHolder
    {
        public static Dictionary<int, ObservableCollection<Problem>> SelectedProblems = new Dictionary<int, ObservableCollection<Problem>>();
        public static Dictionary<int, ObservableCollection<Resource>> SelectedResources = new Dictionary<int, ObservableCollection<Resource>>();
        public static Dictionary<int, ObservableCollection<Target>> SelectedTargets = new Dictionary<int, ObservableCollection<Target>>();
        public static Dictionary<int, ObservableCollection<Measure>> SelectedMeasures = new Dictionary<int, ObservableCollection<Measure>>();
    }
}
