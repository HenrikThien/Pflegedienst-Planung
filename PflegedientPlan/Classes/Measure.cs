using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan.Classes
{
    public enum FrequencyType
    {
        DAILY = 1,
        WEEKLY = 2
    }

    public class Measure
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }
        public int RealListIndex { get; set; }
        public int ActivityId { get; set; }
        public int Frequency { get; set; }
        public FrequencyType FrequencyType { get; set; }

        public string DescriptionString
        {
            get
            {
                return Description + " - Häufigkeit: " + Frequency + " | " + FrequencyTypeToString;
            }
        }

        public string FrequencyTypeToString
        {
            get
            {
                return (FrequencyType == FrequencyType.DAILY) ? "Täglich" : "Wöchentlich";
            }
            set
            {
                var type = value;

                if (type.ToLower() == "täglich")
                    FrequencyType = FrequencyType.DAILY;
                else if (type.ToLower() == "wöchentlich")
                    FrequencyType = FrequencyType.WEEKLY;
            }
        }
    }
}
