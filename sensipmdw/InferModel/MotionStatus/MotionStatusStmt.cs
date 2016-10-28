using System;
using System.Collections.Generic;
using System.Text;
using com.espertech.esper.client;

namespace Sensip
{
    class MotionStatusStmt
    {
        private EPStatement statement;

        public MotionStatusStmt(EPAdministrator admin)
        {
            /*string stmt = "select zoneuri, count(distinct objuri) as numObjsPerZone " +
                          "from ObjLocation.win:time(60 sec) "+
                          "group by zoneuri"; */

            string stmt = "select count(distinct objuri) as cnt, zoneuri " +
                          "from ObjLocation.win:time(60 sec) group by zoneuri";

            statement = admin.CreateEPL(stmt);

        }

        public void AddListener(MotionStatusListener listener)
        {
            statement.Events+=listener.Update;
        }
    }
}
