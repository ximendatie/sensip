using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System;
using com.espertech.esper.client;
using com.espertech.esper.events;

namespace Sensip
{
    class MotionStatusListener : UpdateListener
    {
        MainFrm frm;

        public MotionStatusListener(MainFrm f)
        {
            frm = f;
        }

        public void Update(EventBean[] newEvents, EventBean[] oldEvents)
        {
            if (newEvents != null)
            {
                //output the total number and objects
                string zoneuri = (string)newEvents[0].Get("zoneuri");
                //string numObjsPerZone = (string)newEvents[0].Get("objuri");
                Int64 cnt = (Int64)newEvents[0].Get("cnt");
            }
        }
    }
}
