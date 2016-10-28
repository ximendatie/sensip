
using System;

namespace Sensip
{
    /// <summary>
    /// Summary description for ObjLocation
    /// </summary>
    public class ObjLocation
    {
        /* private ElemType elemType;
         private string objuri;
         private string zoneuri;
         private string isStill;

         private DateTime dtime;*/
        public ElemType elemType { get; private set; }
        public string objuri { get; private set; }
        public string zoneuri { get; private set; }
        public DateTime dtime { get; private set; }
        public string isStill { get; private set; }
        public ObjLocation(string ouri, string zuri, ElemType etype, DateTime time, string still)
        {
            this.objuri = ouri;
            this.zoneuri = zuri;
            this.dtime = time;
            this.isStill = still;
            this.elemType = etype;
        }
        public DateTime getDtime() { return dtime; }
        public string getObjuri() { return objuri; }
        public string getzoneuri() { return zoneuri; }
        public ElemType getElemType() { return elemType; }

    }

}
