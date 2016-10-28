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
    public class ZoneHumidityListener
    {
        MainFrm frm;

        public ZoneHumidityListener(MainFrm f)
        {
            frm = f;
        }

        public void Update(object sender, UpdateEventArgs e)
        {
            if (e.NewEvents != null)
            {
               // foreach (EventBean @event in e.NewEvents)
                //{
                    string objuri = (string)e.NewEvents[0].Get("objuri");
                    double hm = (double)e.NewEvents[0].Get("humidity");
                    MainFrm.rtDataManager.WriteRTDataRow(objuri, 0, "Humidity", hm.ToString());

              //  }
            }

        }
       

    }
}