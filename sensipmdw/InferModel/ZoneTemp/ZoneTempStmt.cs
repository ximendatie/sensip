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
    public class ZoneTempStmt
    {
        private EPStatement statement;

        public ZoneTempStmt(EPAdministrator admin)
        {
            /*string stmt = "select zoneuri, count(distinct objuri) as numObjsPerZone " +
                          "from ObjLocation.win:time(60 sec) "+
                          "group by zoneuri"; */

            string stmt = "select max(dtime), objuri, temperature " +
                          "from ZoneTemperature.win:time(60 sec) group by objuri";
            statement = admin.CreateEPL(stmt);

        }

        public void AddListener(ZoneTempListener listener)
        {
            statement.Events += listener.Update;
        }
    }
}
