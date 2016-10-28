using System;
using System.Collections.Generic;
using System.Text;

namespace Sensip
{
    class ReadRate
    {
        private string objuri;
        private double rrate;

        public string getObjuri() { return objuri; }
        public double getRrate() { return rrate; }

        public ReadRate(string _objuri, double _rrate)
        {
            this.objuri = _objuri;
            this.rrate = _rrate;
        }

    }
}
