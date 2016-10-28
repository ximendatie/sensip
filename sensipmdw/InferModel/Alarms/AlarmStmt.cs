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
    public class AlarmStmt
    {
        private EPStatement statement;

        public AlarmStmt(EPAdministrator admin)
        {
            /*string stmt = "select zoneuri, count(distinct objuri) as numObjsPerZone " +
                          "from ObjLocation.win:time(60 sec) "+
                          "group by zoneuri"; */

            /*string stmt = "select max(dtime), zoneuri, objuri, isStill, elemType " +
                          "from ObjLocation.win:time(60 sec) group by zoneuri, objuri";*/

            string stmt = "SELECT ObjTemperature.objuri as objuri, ObjTemperature.temperature as temperature, ObjLocation.isStill as isStill " +
                        "FROM ObjTemperature.win:length(1),ObjLocation.win:length(1) " +
                        "WHERE (ObjTemperature.temperature > 26) AND (ObjLocation.isStill = 'M' AND ObjTemperature.objuri = ObjLocation.objuri) ";

            statement = admin.CreateEPL(stmt);

        }

        public void AddListener(AlarmListener listener)
        {
            statement.Events+=listener.Update;
        }
    }
}
