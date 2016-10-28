/*
 * computes the total number of objects per zone within the last 60 seconds
 * computes what are those objects
 */
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
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class ZoneHumidityListener : UpdateListener
    {
        MainFrm frm;

        public ZoneHumidityListener(MainFrm f)
        {
            frm = f;
        }

        public void Update(EventBean[] newEvents, EventBean[] oldEvents)
        {
            if (newEvents != null)
            {
                //output the total number and objects

                for (int i = 0; i < newEvents.GetLength(0); i++)
                {
                    string objuri = (string)newEvents[i].Get("objuri");
                    double hm = (double)newEvents[i].Get("humidity");
                    MainFrm.rtDataManager.WriteRTDataRow(objuri, 0, "Humidity", hm.ToString());
                        
                }
               
            }
        }

    }
}
