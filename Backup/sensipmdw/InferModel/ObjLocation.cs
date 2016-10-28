
using System;

namespace Sensip
{
    /// <summary>
    /// Summary description for ObjLocation
    /// </summary>
    public class ObjLocation
    {
        private ElemType elemType;
        private string objuri;
        private string zoneuri;
        private string isStill;

        private DateTime dtime;

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
        public string getZoneuri() { return zoneuri; }
        public string getIsStill() { return isStill; }
        public ElemType getElemType() { return elemType; }
    }

}
