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
        WEEKLY = 2,
        ALWAYS = 3
    }

    public class Measure : iSuperItem
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
        public int Frequency { get; set; }
        
        public void SetSuperFrequency()
        {
            base.SuperFrequency = (FrequencyType == Classes.FrequencyType.DAILY || FrequencyType == Classes.FrequencyType.WEEKLY) ? Frequency + "x " + FrequencyTypeToString : FrequencyTypeToString;
        }

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
                return (FrequencyType == FrequencyType.DAILY) ? "Täglich" : (FrequencyType == FrequencyType.WEEKLY) ? "Wöchentlich" : "Immer";
            }
            set
            {
                var type = value;

                if (type.ToLower() == "täglich")
                    FrequencyType = FrequencyType.DAILY;
                else if (type.ToLower() == "wöchentlich")
                    FrequencyType = FrequencyType.WEEKLY;
                else
                    FrequencyType = FrequencyType.ALWAYS;
            }
        }
    }
}
