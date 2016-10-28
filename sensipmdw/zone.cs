using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Sensip
{
    class zone : ObjBase
    {
        public zone()
        {
        }

        //private string _FullName;
        private int _FullNameIndex;

        private string _Description;
        
        //private string _RFIDReader;
        private int _RFIDReaderIndex;

        private string _TemperatureSensor;
        private string _HimiditySensor;

        private string _RFIDTag;



        [CategoryAttribute("Information"), DescriptionAttribute("the name of zone"),
       TypeConverter(typeof(PropertyGridZonesItem))]
        public int FullName
        {
            get
            {
                return _FullNameIndex;
            }
            set
            {

                _FullNameIndex = value;
            }
        }
        
        

        [CategoryAttribute("Information"), DescriptionAttribute("the description of zone"),
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

        [CategoryAttribute("Sensors"), DescriptionAttribute("the RFID Reader of zone"),
        TypeConverter(typeof(PropertyGridRFIDReadersItem))]
        public int RFIDReader
        {
            get
            {
                return _RFIDReaderIndex;
            }
            set
            {

                _RFIDReaderIndex = value;
            }
        }

        [CategoryAttribute("Sensors"), DescriptionAttribute("the RFID Tag of zone"),
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
        public string HimiditySensor
        {
            get
            {
                return _HimiditySensor;
            }
            set
            {
                _HimiditySensor = value;
            }
        }


    }
}
