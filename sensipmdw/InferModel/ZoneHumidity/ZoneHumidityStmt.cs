/*
 * computes the total number of objects per zone within the last 60 seconds
 * computes what are those objects
 */

using System;
using com.espertech.esper.client;

namespace Sensip
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class ZoneHumidityStmt
    {
        private EPStatement statement;

        public ZoneHumidityStmt(EPAdministrator admin)
        {
            string stmt = "select max(dtime), objuri, humidity " +
                          "from ZoneHumidity.win:time(10 sec) group by objuri";

            statement = admin.CreateEPL(stmt);

        }

        public void AddListener(ZoneHumidityListener listener)
        {
            statement.Events += listener.Update;
        }
    }
}
