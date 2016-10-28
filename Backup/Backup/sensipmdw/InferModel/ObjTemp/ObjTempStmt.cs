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
    public class ObjTempStmt
    {
        private EPStatement statement;

        public ObjTempStmt(EPAdministrator admin)
        {
            /*string stmt = "select zoneuri, count(distinct objuri) as numObjsPerZone " +
                          "from ObjLocation.win:time(60 sec) "+
                          "group by zoneuri"; */

            string stmt = "select max(dtime), objuri, temperature, elemType " +
                          "from ObjTemperature.win:time(10 sec) group by objuri";
            statement = admin.CreateEPL(stmt);

        }

        public void AddListener(UpdateListener listener)
        {
            statement.AddListener(listener);
        }
    }
}
