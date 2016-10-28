using System;
using System.Collections.Generic;
using System.Text;

using com.espertech.esper.client;
using com.espertech.esper.client.time;
using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.support;
using com.espertech.esper.support.util;
using log4net;

namespace Sensip
{
    class CEPManager
    {
        //obj location
        public static EPServiceProvider epService_ObjLoc = null;
        public ZoneObjsListener listener_ObjLoc = null;

        public static EPServiceProvider epService_ObjTemp = null;
        public ObjTempListener listener_ObjTemp = null;

        public static EPServiceProvider epService_ZoneTemp = null;
        public ZoneTempListener listener_ZoneTemp = null;

        public static EPServiceProvider epService_ZoneHumidity = null;
        public ZoneHumidityListener listener_ZoneHumidity = null;

        public static EPServiceProvider epService1 = null;
        public AlarmListener listener_Alarm = null;

        //obj temp ;
        MainFrm frm;

        public CEPManager(MainFrm f)
        {
            frm = f;
        }

        public void CloseCEP()
        {
            epService_ObjLoc.Destroy();
            epService_ObjTemp.Destroy();
            epService_ZoneTemp.Destroy();
            epService_ZoneHumidity.Destroy();
        }

        public void setUp()
        {
            Configuration configuration = new Configuration();
            configuration.AddEventTypeAlias("ObjLocation", typeof(ObjLocation).FullName);
            configuration.AddEventTypeAlias("ZoneHumidity", typeof(ZoneHumidity).FullName);
            configuration.AddEventTypeAlias("ObjTemperature", typeof(ObjTemperature).FullName);
            configuration.AddEventTypeAlias("ZoneTemperature", typeof(ZoneTemperature).FullName);


            listener_ObjLoc = new ZoneObjsListener(frm);
            epService_ObjLoc = EPServiceProviderManager.GetProvider("ZoneObjsEngine", configuration);
            ZoneObjsStmt stmt_ZoneObjs = new ZoneObjsStmt(epService_ObjLoc.EPAdministrator);
            stmt_ZoneObjs.AddListener(listener_ObjLoc);

            listener_ObjTemp = new ObjTempListener(frm);
            epService_ObjTemp = EPServiceProviderManager.GetProvider("ObjTempEngine", configuration);
            ObjTempStmt stmt_ObjTemp = new ObjTempStmt(epService_ObjTemp.EPAdministrator);
            stmt_ObjTemp.AddListener(listener_ObjTemp);

            listener_ZoneTemp = new ZoneTempListener(frm);
            epService_ZoneTemp = EPServiceProviderManager.GetProvider("ZoneTempEngine", configuration);
            ZoneTempStmt stmt_ZoneTemp = new ZoneTempStmt(epService_ZoneTemp.EPAdministrator);
            stmt_ZoneTemp.AddListener(listener_ZoneTemp);

            listener_ZoneHumidity = new ZoneHumidityListener(frm);
            epService_ZoneHumidity = EPServiceProviderManager.GetProvider("ZoneHumidityEngine", configuration);
            ZoneHumidityStmt stmt_ZoneHumidity = new ZoneHumidityStmt(epService_ZoneHumidity.EPAdministrator);
            stmt_ZoneHumidity.AddListener(listener_ZoneHumidity);

            //cep demo
            listener_Alarm = new AlarmListener(frm);
            epService1 = EPServiceProviderManager.GetProvider("Alarm1Engine", configuration);
            AlarmStmt stmt_Alarm = new AlarmStmt(epService1.EPAdministrator);
            stmt_Alarm.AddListener(listener_Alarm);
        }
    }
}
