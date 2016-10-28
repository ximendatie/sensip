using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Sensip
{
    class people : ObjBase
    {
        public people()
        {
        }

        private string _FullName;
        private string _LastName;
        private string _FirstName;
        private int _Age;
        private int _IsFemale;//0: male, 1: female
        private string _Description;

        private string _RFIDTag;
        private string _TemperatureSensor;
        private string _BloodPressureSensor;
        private string _HeartRateSensor;

        [CategoryAttribute("Information"), DescriptionAttribute("the name of people"),
        DefaultValueAttribute(" ")]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
            }
        }

        [CategoryAttribute("Information"), DescriptionAttribute("the name of people"),
        DefaultValueAttribute(" ")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }


        [CategoryAttribute("Information"), DescriptionAttribute("the name of people"),
        DefaultValueAttribute(" ")]
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                _LastName = value;
            }
        }

        [CategoryAttribute("Information"), DescriptionAttribute("the name of people"),
        DefaultValueAttribute(" ")]
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                _FirstName = value;
            }
        }


        [CategoryAttribute("Information"), DescriptionAttribute("the age of people"),
        DefaultValueAttribute(" ")]
        public int Age
        {
            get
            {
                return _Age;
            }
            set
            {
                _Age = value;
            }
        }


        [CategoryAttribute("Information"), DescriptionAttribute("the genda of people"),
        DefaultValueAttribute(" ")]
        public int IsFemale
        {
            get
            {
                return _IsFemale;
            }
            set
            {
                _IsFemale = value;
            }
        }


        [CategoryAttribute("Sensors"), DescriptionAttribute("the RFID Tag of people"),
        DefaultValueAttribute(" ")]
        public string RFIDTag
        {
            get
            {
                return _RFIDTag;
            }
            set
            {
                _RFIDTag = value;
            }
        }

        [CategoryAttribute("Sensors"), DescriptionAttribute("the TemperatureSensor of people"),
        DefaultValueAttribute(" ")]
        public string TemperatureSensor
        {
            get
            {
                return _TemperatureSensor;
            }
            set
            {
                _TemperatureSensor = value;
            }
        }

        [CategoryAttribute("Sensors"), DescriptionAttribute("the BloodPressureSensor of people"),
        DefaultValueAttribute(" ")]
        public string BloodPressureSensor
        {
            get
            {
                return _BloodPressureSensor;
            }
            set
            {
                _BloodPressureSensor = value;
            }
        }

        [CategoryAttribute("Sensors"), DescriptionAttribute("the HeartRateSensor of people"),
        DefaultValueAttribute(" ")]
        public string HeartRateSensor
        {
            get
            {
                return _HeartRateSensor;
            }
            set
            {
                _HeartRateSensor = value;
            }
        }




    }
}
