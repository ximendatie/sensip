using System;
using System.Collections.Generic;
using System.Text;

namespace Sensip
{
    public class ZoneTemperature
    {
        /*private string objuri;
        private double temperature;
        private DateTime dtime;
        private ElemType elemType;*/
        public ElemType elemType { get; private set; }
        public string objuri { get; private set; }
        public DateTime dtime { get; private set; }
        public double temperature { get; private set; }

        public ZoneTemperature(string ouri, ElemType etype, double temp, DateTime time)
        {
            this.objuri = ouri;
            this.temperature = temp;
            this.dtime = time;
            elemType = etype;
        }

        public DateTime getDtime() { return dtime; }
        public string getObjuri() { return objuri; }
        public double getTemperature() { return temperature; }
        public ElemType getElemType() { return elemType; }

    }
}
