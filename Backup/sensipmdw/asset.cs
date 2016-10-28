using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Sensip
{
    class asset : ObjBase
    {
        public asset()
        {
        }

        private string _FullName;
        private string _Description;
        private string _RFIDTag;

        [CategoryAttribute("Information"), DescriptionAttribute("the name of the asset"),
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

        [CategoryAttribute("Information"), DescriptionAttribute("the name of the asset"),
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


        [CategoryAttribute("Sensors"), DescriptionAttribute("the RFID Tag of the asset"),
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


    }
}
