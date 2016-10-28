using System;
using System.Collections.Generic;
using System.Text;

namespace Sensip
{
        public class ZoneHumidity
        {
           /* private ElemType elemType;
            private string objuri;
            private double humidity;
            private DateTime dtime;*/
            public ElemType elemType { get; private set; }
            public string objuri { get; private set; }
            public DateTime dtime { get; private set; }
            public double humidity { get; private set; }
        public ZoneHumidity(string ouri, ElemType etype, double hum, DateTime time)
            {
                this.objuri = ouri;
                this.humidity = hum;
                this.dtime = time;
                this.elemType = etype;
            }

            public DateTime getDtime() { return dtime; }
            public string getObjuri() { return objuri; }
            public double getHumidity() { return humidity; }
            public ElemType getElemType() { return elemType; }


        }

}
