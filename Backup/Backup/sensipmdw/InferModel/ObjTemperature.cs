using System;
using System.Collections.Generic;
using System.Text;

namespace Sensip
{
    public class ObjTemperature
    {
        private string objuri;
        private double temperature;
        private DateTime dtime;
        private ElemType elemType;

        public ObjTemperature(string ouri, ElemType etype, double temp, DateTime time)
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
