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
    public class ObjTempListener
    {
        MainFrm frm;

        public ObjTempListener(MainFrm f)
        {
            frm = f;
        }

        public void Update(object sender, UpdateEventArgs e)
        {
            if (e.NewEvents != null)
            {
                //foreach (EventBean @event in e.NewEvents)
               // {
                    string objuri = (string)e.NewEvents[0].Get("objuri");
                    double temp = (double)e.NewEvents[0].Get("temperature");
                    ElemType et = (ElemType)(e.NewEvents[0].Get("elemType"));
                    MainFrm.rtDataManager.WriteRTDataRow(objuri, et, "Temperature", temp.ToString());
                    frm.updateChart(objuri, temp);
               // }
            }

        }
       
    }
}
