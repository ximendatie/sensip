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
    public class AlarmListener 
    {
        MainFrm frm;

        public AlarmListener(MainFrm f)
        {
            frm = f;
        }

        int i = 0;

       public void Update(object sender, UpdateEventArgs e)
        {
            if (e.NewEvents != null)
            {

                    double temp = (double)e.NewEvents[0].Get("temperature");
                    string motion = (string)e.NewEvents[0].Get("isStill");

                    if (i >= 0 && i <= 3)
                    {
                        i++;
                        frm.CEPAlarm(temp, motion);
                    }
                    else if (i > 5 && i <= 10)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
            }

        }
       

    }
}
