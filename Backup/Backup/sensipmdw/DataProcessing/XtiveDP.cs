using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms ;
using System.Threading;
using SYRIS_XTIVE_STANDARD_READER_1N;

namespace Sensip
{
    class XtiveDP : DataProcessing<XtiveTag>
    {
        string READER1 = "NE-4100T_1083";
        const int cycleSleepTime = 1000; 

        XtiveTag[] tag_data = new XtiveTag[100];
        int RecordCount = 0;

        private XtiveReader reader1 = new XtiveReader();

        
        bool _isRunnin ;

        private string IPAddr ;

        public XtiveDP(string IPStr)
        {
            IPAddr = IPStr;
            bool res = OpenReader(IPAddr, 4001, "3", "0001");
            _isRunnin = res;
        }

        private bool OpenReader(string reader_ip, int port, string reader_automode, string reader_id)
        {
            try
            {
                int reader_port = port;
                int readerautomode = Int32.Parse(reader_automode, System.Globalization.NumberStyles.HexNumber);
                READER1 = reader_id;

                reader1.Open(reader_ip, reader_port, readerautomode, READER1);
                reader1.CMDReaderInit(READER1);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CheckStatus()
        {
            return _isRunnin;
        }



        public override void Producer()
        {
            while (!_bStop)
            {
                bool t = reader1.LoadTagData(ref tag_data, ref RecordCount) ;

                // Showing in the list box
                if (RecordCount != 0)
                {
                    int count = RecordCount;
                    int record_ptr = 0;

                    while (count > 0)
                    {
                        ProducerData(tag_data[record_ptr]);
                        count--;
                        record_ptr++;
                    }
                }

                Thread.Sleep(cycleSleepTime);
            }
            if (reader1.IsConnected)
                reader1.Close();
            return;
        }

        public override void Consumer()
        {
            while (!_bStop)
            {
                try
                {
                    XtiveTag tag = ConsumerData();
                    string TAG_ID = tag.UID;

                    if (!StartRFIDFrm.ObjMapping.ContainsKey(TAG_ID))
                        continue;
                    string USER_NAME = StartRFIDFrm.ObjMapping[TAG_ID][0];
                    string ELEM_TYPE = StartRFIDFrm.ObjMapping[TAG_ID][1];
                    if (USER_NAME == "" || ELEM_TYPE == "")
                        continue;

                    string READER_ID = reader1.ReaderID;
                    if (!StartRFIDFrm.ZoneMapping.ContainsKey(READER_ID))
                        continue;
                    string ZONE_NAME = StartRFIDFrm.ZoneMapping[READER_ID];
                    DateTime dtime = tag.Time;

                    //parameters
                    string isStill = "X";
                    bool isActiveAlam = false; //Ö÷¶¯ÐÅºÅ


                    // "MOTION"
                    if ((tag.DI & 0x04) != 0x04)
                        isStill = "M";
                    else
                        isStill = "X";

                    if ((tag.DI & 0x01) != 0x01)
                        isActiveAlam = true;
                    else
                        isActiveAlam = false;


                    switch (ELEM_TYPE)
                    {
                        //this is a zone, we need report the zone temp and humidity
                        case "0":
                            ZoneHumidity ohz = new ZoneHumidity(ZONE_NAME, ElemType.etZone, tag.HM, tag.Time);
                            ZoneTemperature otz = new ZoneTemperature(ZONE_NAME, ElemType.etZone, tag.T1, tag.Time);
                            CEPManager.epService_ZoneHumidity.EPRuntime.SendEvent(ohz);
                            CEPManager.epService_ZoneTemp.EPRuntime.SendEvent(otz);
                            break;
                        //this is a people, we need report the people temp, location, motion status
                        case "1":
                            ObjTemperature otp = new ObjTemperature(USER_NAME, ElemType.etPeople, tag.T2, tag.Time);
                            ObjLocation objlocp = new ObjLocation(USER_NAME, ZONE_NAME, ElemType.etPeople, dtime, isStill);
                            CEPManager.epService_ObjTemp.EPRuntime.SendEvent(otp);
                            CEPManager.epService_ObjLoc.EPRuntime.SendEvent(objlocp);
                            break;
                        //this is a asset, we need report the asset location, motion status
                        case "2":
                            ObjLocation objloca = new ObjLocation(USER_NAME, ZONE_NAME, ElemType.etAsset, dtime, isStill);
                            CEPManager.epService_ObjLoc.EPRuntime.SendEvent(objloca);
                            ObjTemperature tmp = new ObjTemperature(USER_NAME, ElemType.etAsset, tag.T1, tag.Time);
                            CEPManager.epService_ObjTemp.EPRuntime.SendEvent(tmp);
                            //show the CEP demo

                            CEPManager.epService1.EPRuntime.SendEvent(objloca);
                            CEPManager.epService1.EPRuntime.SendEvent(tmp);

                            break;
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            return;
            
        }
        

    }
}
