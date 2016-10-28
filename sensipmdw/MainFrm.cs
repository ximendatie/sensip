using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using WaveLib.AudioMixer;
using Sensip.Common;
using Sensip.Common.CallControl;

using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;

using org.drools.dotnet.compiler;
using org.drools.dotnet.rule;
using org.drools.dotnet;
using languageParser;

using ZedGraph;

namespace Sensip
{
    public partial class MainFrm : Form
    {
        #region Properties

        CEPManager cepManager = null;
        public static RTDataManager rtDataManager = null;

        private Timer tmr = new Timer();  // Refresh Call List
        private SensipResources _resources = null;
        public SensipResources SensipResources
        {
            get { return _resources; }
        }

        private bool _restart = false;
        private bool RestartRequired
        {
            get { return _restart; }
            set { _restart = value; }
        }

        private bool _reregister = false;
        private bool ReregisterRequired
        {
            get { return _reregister; }
            set { _reregister = value; }
        }
        public bool IsInitialized
        {
            get { return SensipResources.StackProxy.IsInitialized; }
        }

        //------------------------
        string strCurrentPath;
        string defaultimage = "\\data\\default.bmp";
        string mapPath = "\\Maps\\RFIDFloor.svg";
        string selectedID = "";
        int seletedType = 0; //0: room, 1: obj, -1: none

        //------------------------

        StartRFIDFrm RFIDFrm = null;


        #endregion

        #region MapManager
        public AxSVGACTIVEXLib.AxSVGCtl getMapView()
        {
            return mapView;
        }

        public void setMapPath()
        {
            string newMapPath = strCurrentPath + mapPath;
            MessageBox.Show(newMapPath);/////////////////////////////////////////////
            if (File.Exists(newMapPath))
            {
                mapView.setSrc(newMapPath);///现在的问题所在
            }
            else
            {
                MessageBox.Show("Error", "No Map Exists!") ;  
            }
        }

        private void unSelectePrvious()
        {
            if (seletedType == 0)
            { setRoomUnselected(selectedID); }
            else if (seletedType == 1)
            { setObjUnselected(selectedID); }
            else
            { }
        }

        private void setRoomSelected(string id)
        {
            string selectedStyle = "fill:blue;stroke:black;stroke-width:5;fill-opacity:0.5;stroke-opacity:0.9";

            object doc = (object)mapView.getSVGDocument();
            object element = (object)doc.GetType().InvokeMember("getElementById", BindingFlags.InvokeMethod, null, doc, new object[] { id });
            if (element == null)
            {
                return;
            }
            element.GetType().InvokeMember("setAttribute", BindingFlags.InvokeMethod,
                null, element, new object[] { "style", selectedStyle });
            
            selectedID = id;
            seletedType = 0;
        }

        private void setRoomUnselected(string id)
        {
            string unselectedStyle = "fill:white;stroke:black;stroke-width:5;fill-opacity:0.5;stroke-opacity:0.9";

            object doc = (object)mapView.getSVGDocument();
            object element = (object)doc.GetType().InvokeMember("getElementById", BindingFlags.InvokeMethod, null, doc, new object[] { id });
            if (element == null)
            {
                return;
            }
            element.GetType().InvokeMember("setAttribute", BindingFlags.InvokeMethod,
                null, element, new object[] { "style", unselectedStyle });

            selectedID = "";
            seletedType = -1;

        }

        /*
         * id : the id of the obj
         * objType: 0: people, 1:asset
         * MotionStatus: 0: still, 1: moving
         */


        private void setObjSelected(string id, ElemType objType, MotionStatus motionStatus)
        {
            string selectedStyle;
            if (objType == ElemType.etPeople)
            {
                selectedStyle = "fill:green;stroke:black;stroke-width:5;fill-opacity:0.5;stroke-opacity:0.9";
            }
            else
                selectedStyle = "fill:red;stroke:black;stroke-width:5;fill-opacity:0.5;stroke-opacity:0.9";

            string nid;
            if (motionStatus == MotionStatus.msStill)
            {
                nid = "c" + id + "_0";
            }
            else
            {
                nid = "c" + id + "_d";
            }

            object doc = (object)mapView.getSVGDocument();
            object element = (object)doc.GetType().InvokeMember("getElementById", BindingFlags.InvokeMethod, null, doc, new object[] { nid });
            if (element == null)
            {
                return;
            }
            element.GetType().InvokeMember("setAttribute", BindingFlags.InvokeMethod,
                null, element, new object[] { "style", selectedStyle });

            selectedID = id;
            seletedType = 1;

        }

        private void setObjUnselected(string id)
        {
            //still
            string nid = "c" + id + "_0";
            string unselectedStyle = "fill:none;stroke:none;stroke-width:0;fill-opacity:0.5;stroke-opacity:0.9";

            object doc = (object)mapView.getSVGDocument();
            object element = (object)doc.GetType().InvokeMember("getElementById", BindingFlags.InvokeMethod, null, doc, new object[] { nid });
            if (element == null)
            {
                return;
            }
            element.GetType().InvokeMember("setAttribute", BindingFlags.InvokeMethod,
                null, element, new object[] { "style", unselectedStyle });

            //moving
            nid = "c" + id + "_d";
            element = (object)doc.GetType().InvokeMember("getElementById", BindingFlags.InvokeMethod, null, doc, new object[] { nid });
            if (element == null)
            {
                return;
            }
            element.GetType().InvokeMember("setAttribute", BindingFlags.InvokeMethod,
                null, element, new object[] { "style", unselectedStyle });


            //-------
            selectedID = "";
            seletedType = -1;
        }

        #endregion

        #region DataBase Management

        public MySqlConnection conn = null;

        private bool ConnectDB()
        {
            
            if (conn != null && conn.State == ConnectionState.Open)
                return true;

            string host = Properties.Settings.Default.MySqlHost;
            string database = Properties.Settings.Default.MySqlDB;
            string uname = Properties.Settings.Default.MySqlUser;
            string upwd = Properties.Settings.Default.MySqlpwd;
            //string constr = "server=localhost;User Id=root;Database=opensips";
            string constr = "server=localhost;User Id=root;Database=test";/////////////////////////////////////

            conn = new MySqlConnection(constr);
            //msg
            try
            {
                   
                conn.Open();
                MessageBox.Show("Just OK", "Fine");//////////////////////////////////////
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot Connect Database!", "Error");
                return false;
            }
        }

        private void DisconnectDB()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        //UserName---> FullName
        bool needUpdate = true;
        public static Dictionary<string, string> UNFNMapping = new Dictionary<string, string>();

        public string UserName2FullNameForZones(string UserName)
        {
            if (needUpdate == false)
            {
                try
                {
                    return UNFNMapping[UserName];
                }
                catch (Exception ee)
                {
                    return "";
                }
            }
            if (conn.State != ConnectionState.Open)
                return "";
            try
            {
                UNFNMapping.Clear();
                String sqlstr = "SELECT FullName, UserName FROM z_zones";

                MySqlCommand command = new MySqlCommand(sqlstr, conn);
                command.CommandType = CommandType.Text;
                MySqlDataReader reader = command.ExecuteReader();
                string fn;
                string un;
                while (reader.Read())
                {
                    fn = reader.GetString(0);
                    un = reader.GetString(1);
                    UNFNMapping.Add(un, fn);
                }

                command.Dispose();
                needUpdate = false;
                return UNFNMapping[UserName];
            }
            catch (Exception ee)
            {
                needUpdate = true;
                return "";
            }

        }

                

        #endregion

        
        #region Mapping Table Management
        private void UpdateMappingTable()
        {
            StartRFIDFrm.ObjMapping.Clear();
            StartRFIDFrm.ZoneMapping.Clear();

            String sqlstr = "SELECT z_accounts.UserName, RFIDReader FROM z_accounts, z_zones WHERE z_accounts.UserName = z_zones.UserName";
            MySqlCommand command = new MySqlCommand(sqlstr, conn);
            command.CommandType = CommandType.Text;
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string UserName = reader.GetString(0);
                string RFIDReader = reader.GetString(1) ;
                StartRFIDFrm.ZoneMapping.Add(RFIDReader, UserName);
            }
            command.Dispose();

            sqlstr = "SELECT z_accounts.UserName, ElemType, RFIDTag FROM z_accounts, z_people WHERE z_accounts.UserName = z_people.UserName";
            MySqlCommand command1 = new MySqlCommand(sqlstr, conn);
            command1.CommandType = CommandType.Text;
            MySqlDataReader reader1 = command1.ExecuteReader();
            if (reader1.Read())
            {
                string UserName = reader1.GetString(0);
                string ElemType = reader1.GetString(1);
                string RFIDTag = reader1.GetString(2);
                StartRFIDFrm.ObjMapping.Add(RFIDTag, new string[] { UserName, ElemType });
            }
            command1.Dispose();

            sqlstr = "SELECT z_accounts.UserName, ElemType, RFIDTag FROM z_accounts, z_assets WHERE z_accounts.UserName = z_assets.UserName";
            MySqlCommand command2 = new MySqlCommand(sqlstr, conn);
            command2.CommandType = CommandType.Text;
            MySqlDataReader reader2 = command1.ExecuteReader();
            if (reader2.Read())
            {
                string UserName = reader2.GetString(0);
                string ElemType = reader2.GetString(1);
                string RFIDTag = reader2.GetString(2);
                StartRFIDFrm.ObjMapping.Add(RFIDTag, new string[] { UserName, ElemType });
            }
            command2.Dispose();

            
        }


        #endregion

        #region VoiceManager
        VoiceManager VM;
        string SpeakStr = "Please Select the Entity";

        #endregion

        #region MessageManager
        Parser pp = new Parser();
        //Parser pp;

        #endregion

        #region CallManager

        //////////////////////////////////////////////////////////////////////////////////////
        /// Register callbacks and synchronize threads
        /// 
        delegate void DRefreshForm();
        delegate void DCallStateChanged(int sessionId);
        delegate void MessageReceivedDelegate(string from, string message);
        delegate void BuddyStateChangedDelegate(int buddyId, int status, string text);
        delegate void DMessageWaiting(int mwi, string text);
        delegate void DIncomingCall(int sessionId, string number, string info);

        void CallManager_IncomingCallNotification(int sessionId, string number, string info)
        {
            if (InvokeRequired)
                this.BeginInvoke(new DIncomingCall(this.OnIncomingCall), new object[] { sessionId, number, info });
            else
                OnIncomingCall(sessionId, number, info);
        }

        public void onCallStateChanged(int sessionId)
        {
            if (InvokeRequired)
                this.BeginInvoke(new DRefreshForm(this.RefreshForm));
            else
                RefreshForm();
        }

        public void onMessageReceived(string from, string message)
        {
            if (InvokeRequired)
                this.BeginInvoke(new MessageReceivedDelegate(this.MessageReceived), new object[] { from, message });
            else
                MessageReceived(from, message);
        }

        public void onBuddyStateChanged(int buddyId, int status, string text)
        {
            /*if (InvokeRequired)
                this.BeginInvoke(new BuddyStateChangedDelegate(this.BuddyStateChanged), new object[] { buddyId, status, text });
            else
                BuddyStateChanged(buddyId, status, text);*/
        }

        public void onAccountStateChanged(int accId, int accState)
        {
            if (InvokeRequired)
                this.BeginInvoke(new DRefreshForm(this.RefreshForm));
            else
                RefreshForm();
        }

        public void onMessageWaitingIndication(int mwi, string text)
        {
            if (InvokeRequired)
                this.BeginInvoke(new DMessageWaiting(this.MessageWaiting), new object[] { mwi, text });
            else
                MessageWaiting(mwi, text);
        }

        private void OnIncomingCall(int sessionId, string num, string info)
        {
            //notifyIcon.ShowBalloonTip(30, "Sensip Softphone", "Incoming call from " + number + "!", ToolTipIcon.Info);
            
            Dictionary<int, IStateMachine> callList = SensipResources.CallManager.CallList;

            foreach (KeyValuePair<int, IStateMachine> kvp in callList)
            {
                string number = kvp.Value.CallingNumber;
                if (number != num)
                    continue;

                string name = kvp.Value.CallingName;
                SensipResources.CallManager.onUserAnswer(kvp.Value.Session);
            }
        }

        public void UpdateCallTimeout(object sender, EventArgs e)
        {
            //---
        }

        private void MessageReceived(string from, string message)
        {
            // extract buddy ID
            CBuddyRecord buddy;
            ElemType et = ElemType.etOther;
            string result = "";
            string buddynum = "-1";
            string userName = "-1";

            string buddyId = parseFrom(from);
            buddynum = buddyId;
            int id = CBuddyList.getInstance().getBuddyId(buddyId);
            if (id >= 0)
            {
                //_buddyId = id;        
                buddy = CBuddyList.getInstance()[id];
                buddynum = buddy.Number;
                //userName = buddy.

            }
            
            message = message.Trim();
            string InquireObject = "";

            message = message.Trim();
            string[] parts = message.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                message = parts[0];
            }
            else if (parts.Length == 2)
            {
                InquireObject = parts[0];
                message = parts[1];
            }
            else
            {
                message = "";
            }

            pp.setInput(message);
            pp.doParsing();
            string msg = pp.getOutput();
            if (msg == null)
                msg = "";
            //
            if (msg == "Hello!")
            {
                SensipResources.Messenger.sendMessage(buddynum, "Welcome, Plz input your query as UserName:AttributeName");
            }
            else if (msg == "Goodbye!")
            {
                SensipResources.Messenger.sendMessage(buddynum, "See you next time");
            }
            else if (msg == "")
            {
                SensipResources.Messenger.sendMessage(buddynum, "Cannot be recognized, input again");
            }
            else
            {
                result = rtDataManager.GetFieldInfo(InquireObject, msg);
                if (result == "")
                {
                    SensipResources.Messenger.sendMessage(buddynum, "Cannot find this information");
                }
                else
                {
                    SensipResources.Messenger.sendMessage(buddynum, "The Information You Are Looking For Is: " + result);
                }
            }

      }

        private string parseFrom(string from)
        {
            string number = from.Replace("<sip:", "");

            int atPos = number.IndexOf('@');
            if (atPos >= 0)
            {
                number = number.Remove(atPos);
                int first = number.IndexOf('"');
                if (first >= 0)
                {
                    int last = number.LastIndexOf('"');
                    number = number.Remove(0, last + 1);
                    number = number.Trim();
                }
            }
            else
            {
                int semiPos = number.IndexOf(';');
                if (semiPos >= 0)
                {
                    number = number.Remove(semiPos);
                }
                else
                {
                    int colPos = number.IndexOf(':');
                    if (colPos >= 0)
                    {
                        number = number.Remove(colPos);
                    }
                }
            }
            return number;
        }

        private void MessageWaiting(int mwi, string info)
        {
            string[] parts = info.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string vmaccount = "";
            string noofvms = "";

            if (parts.Length == 3)
            {
                int index = parts[1].IndexOf("Message-Account: ");
                if (index == 0)
                {
                    vmaccount = parts[1].Substring("Message-Account: ".Length);
                }

                if (parts[2].IndexOf("Voice-Message: ") >= 0)
                {
                    noofvms = parts[2].Substring("Voice-Message: ".Length);
                }

            }

            if (mwi > 0)
                toolStripStatusLabelMessages.Text = "Message Waiting: " + noofvms + " - Account: " + vmaccount;
            else
                toolStripStatusLabelMessages.Text = "No Messages!";

        }

        #endregion

        #region Presence Manager

        /*   Complex Message Format
         *   <PeopleInfo>
                <UserName>000</UserName>
                <Location>301K</Location>
                <Temperature>15</Temperature>
            </PeopleInfo>
         */
         
        public int PublishComplexStatus(int accId, string Info)
        {
            return SensipResources.Messenger.setComplexStatus(accId, EUserStatus.COMPLEX, Info);
            
        }

        #endregion

        public MainFrm()
        {
            InitializeComponent();

            Properties.Settings.Default.Reload();

            // Check if settings upgrade needed?
            if (Properties.Settings.Default.cfgUpdgradeSettings == true)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.cfgUpdgradeSettings = false;
            }
            // Create resource object containing SensipSdk and other Sensip related data
            _resources = new SensipResources(this);

        }

        private void RefreshForm()
        {
            if (IsInitialized)
            {
                // Update Call Status
                UpdateCallLines(-1);

                // Update Call Register
                UpdateCallRegister();

                // Update account list
                UpdateAccountList(0);
            }
        }

        private void UpdateCallLines(int sessionId)
        {
            listViewCallLines.Items.Clear();

            try
            {
                // get entire call list
                Dictionary<int, IStateMachine> callList = SensipResources.CallManager.CallList;

                foreach (KeyValuePair<int, IStateMachine> kvp in callList)
                {
                    string number = kvp.Value.CallingNumber;
                    string name = kvp.Value.CallingName;

                    string duration = kvp.Value.Duration.ToString();
                    if (duration.IndexOf('.') > 0) duration = duration.Remove(duration.IndexOf('.')); // remove miliseconds
                    // show name & number or just number
                    string display = name.Length > 0 ? name + " / " + number : number;
                    string stateName = kvp.Value.StateId.ToString();
                    if (SensipResources.CallManager.Is3Pty) stateName = "CONFERENCE";
                    ListViewItem lvi = new ListViewItem(new string[] { stateName, display, duration });

                    lvi.Tag = kvp.Value;
                    listViewCallLines.Items.Add(lvi);
                    lvi.Selected = true;
                }

                if (callList.Count > 0)
                {
                    // control refresh timer
                    tmr.Start();
                }

            }
            catch (Exception e)
            {
                // TODO!!!!!!!!!!! Sychronize SHARED RESOURCES!!!!
            }
        }

        private void UpdateCallRegister()
        {
//---            
        }



        private void UpdateAccountList(int seletedindex)
        {
            
            listViewAccounts.Items.Clear();

            for (int i = 0; i < SensipResources.Configurator.Accounts.Count; i++)
            {
                IAccount acc = SensipResources.Configurator.Accounts[i];
                string name;

                if (acc.UserName.Length == 0)
                {
                    name = "--empty--";
                }
                else
                {
                    name = acc.DisplayName;
                }
                // create listviewitem
                ListViewItem item = new ListViewItem(new string[] { name, acc.RegState.ToString(), acc.UserName });

                // mark default account
                if (i == SensipResources.Configurator.DefaultAccountIndex)
                {
                    // Mark default account; todo!!! Coloring!
                    
                    item.ImageKey = "middleware16";

                    string label = "";
                    // check registration status
                    if (acc.RegState == 200)
                    {
                        this.Text = acc.UserName + " (" + acc.DisplayName + ")"; ;
                        label = "Registered" + " - " + acc.UserName + " (" + acc.DisplayName + ")";
                        item.BackColor = Color.LightGreen;
                    }
                    else if (acc.RegState == 0)
                    {
                        label = "Trying..." + " - " + acc.UserName;
                    }
                    else
                    {
                        label = "Not registered" + " - " + acc.UserName;
                    }
                    toolStripStatusLabel.Text = label;
                }
                else
                {
                    switch (acc.ElemType)
                    {
                        case 0:
                            item.ImageKey = "zone16";
                            break ;
                        case 1:
                            item.ImageKey = "people16";
                            break ;
                        case 2:
                            item.ImageKey = "asset16";
                            break ;

                    }
                    if (acc.RegState == 200)
                    {
                        item.BackColor = Color.LightGreen;
                    }

                }

                listViewAccounts.Items.Add(item);
            }
            if (seletedindex < SensipResources.Configurator.Accounts.Count)
                listViewAccounts.Items[seletedindex].Selected = true;

        }


        private void ShutdownVoIP()
        {
            try
            {
                if (IsInitialized)
                {
                    //SensipResources.CallLogger.save();
                }
                SensipResources.Configurator.Save();
            }
            catch (Exception e)
            {
                return;
            }
            // shutdown stack
            SensipResources.CallManager.Shutdown();
        }

        private void OpenSettingsForm(object sender, EventArgs e)
        {
            (new SettingsForm(this.SensipResources, false)).ShowDialog();
            RefreshForm();
        }

        private void LoadAudioValues()
        {
            //---
        }

       

        private void MainFrm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            Splash SplashScreen = new Splash();
            SplashScreen.Show();
            SplashScreen.Refresh();

            needUpdate = true;

            SplashScreen.SetStatus("Connecting Database...");

            if (!ConnectDB())
            {
                ServerConfig sc = new ServerConfig();
                if (sc.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.MySqlHost = sc.ServerIP;
                    Properties.Settings.Default.sipAccountAddress = sc.ServerIP;
                    Properties.Settings.Default.Save();

                    if (!ConnectDB())
                    {
                        Environment.Exit(0);
                        SplashScreen.SetStatus("Cannot Connect Database, Exit");
                        return;
                    }
                }
                else
                {
                    Environment.Exit(0);
                    SplashScreen.SetStatus("Cannot Connect Database, Exit");
                    return;
                }
            }

            SplashScreen.SetStatus("Connecting Real-Time Data Table...");
            //load RTTable
            rtDataManager = new RTDataManager();

            strCurrentPath = System.IO.Directory.GetCurrentDirectory().ToString();

            //test database;
            if (!rtDataManager.LoadRTData())
            {
                Environment.Exit(0);
                return;
            }

            SplashScreen.SetStatus("Load Audio Values...");

            LoadAudioValues();
            // Register callbacks from callcontrol
            SensipResources.CallManager.CallStateRefresh += onCallStateChanged;
            SensipResources.CallManager.IncomingCallNotification += new DIncomingCallNotification(CallManager_IncomingCallNotification);
            // Register callbacks from pjsipWrapper
            //SensipFactory.getCommonProxy().CallStateChanged += onTelephonyRefresh;
            SensipResources.Messenger.MessageReceived += onMessageReceived;
            SensipResources.Messenger.BuddyStatusChanged += onBuddyStateChanged;
            SensipResources.Registrar.AccountStateChanged += onAccountStateChanged;
            SensipResources.StackProxy.MessageWaitingIndication += onMessageWaitingIndication;

            // Initialize and set factory for CallManager

            SplashScreen.SetStatus("Initiate Resources...");
            int status = SensipResources.CallManager.Initialize();
            SensipResources.CallManager.CallLogger = SensipResources.CallLogger;

            if (status != 0)
            {
                (new ErrorDialog("Initialize Error", "Init SIP stack problem! \r\nPlease, check configuration and start again! \r\nStatus code " + status)).ShowDialog();
                return;
            }

            SplashScreen.SetStatus("Register Accounts...");
            // initialize Stack
            SensipResources.Registrar.registerAccounts();

            //////////////////////////////////////////////////////////////////////////
            // load settings

            this.UpdateCallRegister();

            this.UpdateAccountList(0);


            SplashScreen.SetStatus("Register Codecs...");
            // scoh::::03.04.2008:::pjsip ISSUE??? At startup codeclist is different as later 
            // set codecs priority...
            // initialize/reset codecs - enable PCMU and PCMA only
            int noOfCodecs = SensipResources.StackProxy.getNoOfCodecs();
            for (int i = 0; i < noOfCodecs; i++)
            {
                string codecname = SensipResources.StackProxy.getCodec(i);
                if (SensipResources.Configurator.CodecList.Contains(codecname))
                {
                    // leave default
                    SensipResources.StackProxy.setCodecPriority(codecname, 128);
                }
                else
                {
                    // disable
                    SensipResources.StackProxy.setCodecPriority(codecname, 0);
                }
            }

            SplashScreen.SetStatus("Init Middleware Account...");
            InitMiddlewareAccount();

            //Update Mapping Table

            SplashScreen.SetStatus("Set CEP Engine...");
            //load cep
            cepManager = new CEPManager(this);
            cepManager.setUp();

            // timer 
            tmr.Interval = 1000;
            tmr.Tick += new EventHandler(UpdateCallTimeout);

            SplashScreen.SetStatus("Set Map Engine...");
            setMapPath();
            mapView.Refresh();

            VM = new VoiceManager();
            setupChart();

            //create startRFID
            RFIDFrm = new StartRFIDFrm(this);


            this.Visible = true;
            SplashScreen.Close();

            SetUpdateTimer(true);
        }


        public void SetUpdateTimer(bool enabled)
        {
            timer1.Enabled = enabled;
        }


        private void myObjectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myObjectComboBox.SelectedIndex == 0)
            {
                ElemPic.Image = bimageList.Images["zone.ico"];
                zone zoneelem = new zone() ;
                propertyGrid1.SelectedObject = zoneelem;
            }
            else if (myObjectComboBox.SelectedIndex == 1)
            {
                ElemPic.Image = bimageList.Images["people.ico"];
                people peopleelem = new people();
                propertyGrid1.SelectedObject = peopleelem;

            }
            else if (myObjectComboBox.SelectedIndex == 2)
            {
                ElemPic.Image = bimageList.Images["asset.ico"];
                asset assetelem = new asset();
                propertyGrid1.SelectedObject = assetelem;
            }
            else
            {
                ElemPic.Image = null;
                propertyGrid1.SelectedObject = null;
            }
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RFIDFrm.StopProcessing();
            System.Threading.Thread.Sleep(1000);
            RFIDFrm.Dispose();
            cepManager.CloseCEP();
            //DisconnectDB();
            ShutdownVoIP();
        }

        private void InitMiddlewareAccount()
        {
            if (SensipResources.Configurator.Accounts.Count == 0)
            {
                (new SettingsForm(this.SensipResources, true)).ShowDialog();
                RefreshForm();
            }

            listViewAccounts.Items[0].Selected = true;
        }

        private void updateRTShows(string UserName, ElemType elemType, string FullName)
        {
            switch (elemType)
            {
                case ElemType.etZone:
                    {
                        DataRow drow = rtDataManager.LoadRTDataRow(UserName, elemType);
                        RowWrapper wrapper = new RowWrapper(drow);
                        wrapper.Exclude.Add("UserName");
                        propertyGrid3.SelectedObject = wrapper;
                        propertyGrid3.PerformLayout();
                        propertyGrid3.ResumeLayout();

                        //map
                        unSelectePrvious();

                        //rtDataManager.WriteRTDataRow(UserName, elemType, "Location", Properties.Settings.Default.cfgZones[FullName]);
                        setRoomSelected(UserName2FullNameForZones(UserName));
                        break;
                    }
                case ElemType.etPeople:
                    {
                        DataRow drow = rtDataManager.LoadRTDataRow(UserName, elemType);
                        RowWrapper wrapper = new RowWrapper(drow);
                        wrapper.Exclude.Add("UserName");
                        propertyGrid3.SelectedObject = wrapper;
                        propertyGrid3.PerformLayout();
                        propertyGrid3.ResumeLayout();

                        unSelectePrvious();
                        //find the location of this people
                        string loc = rtDataManager.GetFieldInfo(UserName, elemType, "Location");
                        string motion = rtDataManager.GetFieldInfo(UserName, elemType, "Motion");

                        MotionStatus mValue = MotionStatus.msStill;
                        if (motion == "X")
                            mValue = MotionStatus.msStill;
                        else
                            mValue = MotionStatus.msMoving;
                        //setObjSelected(string id, int objType, int motionStatus) ;
                        setObjSelected(loc, elemType, mValue);
                        break;
                    }
                case ElemType.etAsset:
                    {
                        DataRow drow = rtDataManager.LoadRTDataRow(UserName, elemType);
                        RowWrapper wrapper = new RowWrapper(drow);
                        wrapper.Exclude.Add("UserName");
                        propertyGrid3.SelectedObject = wrapper;
                        propertyGrid3.PerformLayout();
                        propertyGrid3.ResumeLayout();

                        unSelectePrvious();
                        string loc = rtDataManager.GetFieldInfo(UserName, elemType, "Location");
                        string motion = rtDataManager.GetFieldInfo(UserName, elemType, "Motion");

                        MotionStatus mValue = MotionStatus.msStill;
                        if (motion == "X")
                            mValue = MotionStatus.msStill;
                        else
                            mValue = MotionStatus.msMoving;
                        //setObjSelected(string id, int objType, int motionStatus) ;
                        setObjSelected(loc, elemType, mValue);
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        private void listViewAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewAccounts.SelectedItems.Count > 0)
            {
                if (!ConnectDB()) { return; }

                int index = listViewAccounts.SelectedItems[0].Index;
                IAccount acc = SensipResources.Configurator.Accounts[index];
                if (acc == null)
                {
                    MessageBox.Show("Error, No Account Exist.");
                    // error!!!
                    return;
                }
                AccountSettings asettings = new AccountSettings() ;
                asettings.DisplayName = acc.DisplayName;
                asettings.UserName = acc.UserName;
                asettings.Password = acc.Password;
                asettings.Description = acc.Description;
                asettings.Picture = acc.Picture;
                propertyGrid2.SelectedObject = asettings;

                if (acc.Picture.Trim() != "" && File.Exists(acc.Picture))
                    pictureBox1.ImageLocation = acc.Picture;
                else
                    pictureBox1.ImageLocation = strCurrentPath + defaultimage;


                switch (acc.ElemType)
                {
                    case 0:
                        myObjectComboBox.SelectedIndex = 0;
                        break;
                    case 1:
                        myObjectComboBox.SelectedIndex = 1;
                        break;
                    case 2:
                        myObjectComboBox.SelectedIndex = 2;
                        break;
                    default:
                        myObjectComboBox.SelectedIndex = -1;
                        break;
                }
                myObjectComboBox_SelectedIndexChanged(this, null);

                propertyGrid1.Enabled = false;
                hidePanel.Visible = false;
                myObjectComboBox.Visible = false;
                myObjectComboBoxlabel.Visible = false;
                toolStripButtonApply.Enabled = false;
                toolStripButtonDelete.Enabled = false;


                if (acc.Index == 0)//middleware account
                {
                    hidePanel.Visible = true;
                    hidePanel.Location = myObjectComboBox.Location;
                    ElemPic.Image = bimageList.Images["middleware.ico"];

                }
                else if (acc.AccountName.Length != 0)
                {
                    myObjectComboBox.Visible = true;
                    myObjectComboBoxlabel.Visible = true;
                    propertyGrid1.Enabled = true;
                    toolStripButtonApply.Enabled = true;
                    toolStripButtonDelete.Enabled = true;
                    

                    int elemtype = SensipResources.Configurator.Accounts[index].ElemType;
                    switch (elemtype)
                    {
                        case 0:
                            {
                                zone zoneelem = new zone() ;
                                String sqlstr = "SELECT FullName, Description, RFIDReader, HumiditySensor,TemperatureSensor FROM z_zones WHERE UserName = @username";
                                MySqlParameter par = new MySqlParameter("@username", MySqlDbType.String);
                                par.Value = acc.UserName;
                                MySqlCommand command = new MySqlCommand(sqlstr, conn);
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(par);
                                MySqlDataReader reader = command.ExecuteReader();
                                if (reader.Read())
                                {
                                    zoneelem.FullName = Properties.Settings.Default.cfgZones.IndexOf(reader.GetString(0));
                                    zoneelem.Description = reader.GetString(1);
                                    zoneelem.RFIDReader = Properties.Settings.Default.cfgRFIDReaders.IndexOf(reader.GetString(2));
                                    zoneelem.HimiditySensor = reader.GetString(3);
                                    zoneelem.TemperatureSensor = reader.GetString(4);
                                    propertyGrid1.SelectedObject = zoneelem;
                                }

                                command.Dispose();


                                //real-time data
                                updateRTShows(acc.UserName, (ElemType)elemtype, "");
                                break;
                            }
                        case 1:
                            {
                                people peopleelem = new people();
                                String sqlstr = "SELECT FullName, Description, LastName, FirstName, Age, IsFemale, RFIDTag, TemperatureSensor, BloodPressureSensor, HeartRateSensor FROM z_people WHERE UserName = @username";
                                MySqlParameter par = new MySqlParameter("@username", MySqlDbType.String);
                                par.Value = acc.UserName;
                                MySqlCommand command = new MySqlCommand(sqlstr, conn);
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(par);
                                MySqlDataReader reader = command.ExecuteReader();
                                if (reader.Read())
                                {
                                    peopleelem.FullName = reader.GetString(0);
                                    peopleelem.Description = reader.GetString(1);
                                    peopleelem.LastName = reader.GetString(2);
                                    peopleelem.FirstName = reader.GetString(3);
                                    peopleelem.Age = reader.GetInt32(4);
                                    peopleelem.IsFemale = reader.GetInt32(5);
                                    peopleelem.RFIDTag = reader.GetString(6);
                                    peopleelem.TemperatureSensor = reader.GetString(7);
                                    peopleelem.BloodPressureSensor = reader.GetString(8);
                                    peopleelem.HeartRateSensor = reader.GetString(9);
                                    propertyGrid1.SelectedObject = peopleelem;
                                }
                                command.Dispose();

                                //real-time data
                                updateRTShows(acc.UserName, (ElemType)elemtype, acc.DisplayName);

                                break;
                            }
                        case 2:
                            {
                                asset assetelem = new asset();
                                String sqlstr = "SELECT FullName, Description, RFIDTag FROM z_assets WHERE UserName = @username";
                                MySqlParameter par = new MySqlParameter("@username", MySqlDbType.String);
                                par.Value = acc.UserName;
                                MySqlCommand command = new MySqlCommand(sqlstr, conn);
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(par);
                                MySqlDataReader reader = command.ExecuteReader();
                                if (reader.Read())
                                {
                                    assetelem.FullName = reader.GetString(0);
                                    assetelem.Description = reader.GetString(1);
                                    assetelem.RFIDTag = reader.GetString(2);
                                    propertyGrid1.SelectedObject = assetelem;
                                }
                                command.Dispose();

                                //real-time data
                                updateRTShows(acc.UserName, (ElemType)elemtype, acc.DisplayName);
                                break;
                            }
                    }

                }
                else
                {
                    myObjectComboBox.Visible = true;
                    myObjectComboBoxlabel.Visible = true;
                    myObjectComboBox.SelectedIndex = -1 ;
                    toolStripButtonApply.Enabled = true;
                    propertyGrid1.Enabled = true;
                    ElemPic.Image = null;
                }
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            int index = this.listViewAccounts.SelectedItems[0].Index ;
            if (index >= 0)
            {
                if (!ConnectDB()) { return; }

                IAccount account = SensipResources.Configurator.Accounts[index];
                string oldusername = account.UserName;

                //start verify
                if (
                    ((AccountSettings)(propertyGrid2.SelectedObject)).UserName.Trim() == "" ||
                    ((AccountSettings)(propertyGrid2.SelectedObject)).DisplayName.Trim() == ""
                    )
                {
                    MessageBox.Show("AccountName, UserName, DisplayName are not allowed to be empty.", "Error");
                    return;
                }
                for (int i = 0; i < SensipResources.Configurator.Accounts.Count; i++)
                {
                    if (i == index)
                        continue;
                    IAccount acc = SensipResources.Configurator.Accounts[i];

                    if (acc.UserName.Trim() == ((AccountSettings)(propertyGrid2.SelectedObject)).UserName.Trim())
                    {
                        MessageBox.Show("This UserName is already in use.", "Error");
                        return;
                    }
                    if (acc.DisplayName.Trim() == ((AccountSettings)(propertyGrid2.SelectedObject)).DisplayName.Trim())
                    {
                        MessageBox.Show("This DisplayName is already in use.", "Error");
                        return;
                    }
                }

                //强行认为fullname就是display name！

                //end verify

                //--------------------------------------Save To Local
                account.HostName = SensipResources.Configurator.Accounts[0].HostName;

                account.DisplayName = ((AccountSettings)(propertyGrid2.SelectedObject)).DisplayName.Trim();
                account.Id = ((AccountSettings)(propertyGrid2.SelectedObject)).UserName.Trim();
                account.UserName = ((AccountSettings)(propertyGrid2.SelectedObject)).UserName.Trim();
                account.Password = ((AccountSettings)(propertyGrid2.SelectedObject)).Password.Trim();
                account.Description = ((AccountSettings)(propertyGrid2.SelectedObject)).Description.Trim();
                account.Picture = ((AccountSettings)(propertyGrid2.SelectedObject)).Picture.Trim();
                account.AccountName = account.UserName;
                account.Index = index;
                account.ElemType = myObjectComboBox.SelectedIndex;
                
                account.ProxyAddress = SensipResources.Configurator.Accounts[0].ProxyAddress;
                account.DomainName = SensipResources.Configurator.Accounts[0].DomainName;
                account.TransportMode = SensipResources.Configurator.Accounts[0].TransportMode;


                //move, rename the picture
                if (account.Picture.Trim() != "")
                {
                    try
                    {
                        string FileNewPath = strCurrentPath + "\\data\\" + account.UserName + System.IO.Path.GetExtension(account.Picture);
                        System.IO.File.Copy(account.Picture, FileNewPath, true);
                        account.Picture = FileNewPath;
                    }
                    catch (Exception ex)
                    {
                        account.Picture = strCurrentPath + defaultimage;
                    }
                }
                

                //indicating it is the empty account, so must generate another empty account
                bool isInsert = false;
                if (oldusername.Trim() == "")
                    isInsert = true;


                //--------------------------------------Save To Remote DB
                string sqlstr;
                if (isInsert)
                {
                    sqlstr = "INSERT INTO z_accounts (DisplayName, UserName, Password, ElemType, Description, Picture) VALUES (@displayname, @username, @password, @elemtype, @description, @picture)";
                }
                else
                {
                    sqlstr = "UPDATE z_accounts SET DisplayName=@displayname, Password=@password, ElemType=@elemtype, Description=@description, Picture=@picture WHERE UserName = @username";
                }

                MySqlParameter par2 = new MySqlParameter("@displayname", MySqlDbType.String);
                MySqlParameter par3 = new MySqlParameter("@username", MySqlDbType.String);
                MySqlParameter par4 = new MySqlParameter("@password", MySqlDbType.String);
                MySqlParameter par5 = new MySqlParameter("@elemtype", MySqlDbType.Int16);
                MySqlParameter par6 = new MySqlParameter("@description", MySqlDbType.String);
                MySqlParameter par7 = new MySqlParameter("@picture", MySqlDbType.Byte);

                par2.Value = SensipResources.Configurator.Accounts[index].DisplayName;
                par3.Value = SensipResources.Configurator.Accounts[index].UserName;
                par4.Value = SensipResources.Configurator.Accounts[index].Password;
                par5.Value = SensipResources.Configurator.Accounts[index].ElemType;
                par6.Value = SensipResources.Configurator.Accounts[index].Description;

                string pathName = SensipResources.Configurator.Accounts[index].Picture;
                if (pathName != "")
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(pathName);
                    System.IO.FileStream fs = new System.IO.FileStream(pathName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    byte[] buffByte = new byte[fs.Length];
                    fs.Read(buffByte, 0, (int)fs.Length);
                    fs.Close();
                    fs = null;

                    par7.Value = buffByte;
                }
                else
                    par7.Value = "";

                MySqlCommand cmd = new MySqlCommand(sqlstr, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(par2);
                cmd.Parameters.Add(par3);
                cmd.Parameters.Add(par4);
                cmd.Parameters.Add(par5);
                cmd.Parameters.Add(par6);
                cmd.Parameters.Add(par7);
                cmd.ExecuteNonQuery();

                //------------
                string sqlstr1;

                switch (SensipResources.Configurator.Accounts[index].ElemType)
                {
                    case 0:
                        {
                            if (isInsert) sqlstr1 = "INSERT INTO z_zones (UserName, FullName, Description, RFIDReader, HumiditySensor,TemperatureSensor) VALUES (@UserName, @FullName, @Description, @RFIDReader, @HumiditySensor,@TemperatureSensor)";
                            else sqlstr1 = "UPDATE z_zones SET UserName=@UserName, FullName=@FullName, Description=@Description, RFIDReader=@RFIDReader, HumiditySensor=@HumiditySensor,TemperatureSensor=@TemperatureSensor";
                            MySqlParameter spar2 = new MySqlParameter("@UserName", MySqlDbType.String);
                            MySqlParameter spar3 = new MySqlParameter("@FullName", MySqlDbType.String);
                            MySqlParameter spar4 = new MySqlParameter("@Description", MySqlDbType.String);
                            MySqlParameter spar5 = new MySqlParameter("@RFIDReader", MySqlDbType.String);
                            MySqlParameter spar6 = new MySqlParameter("@HumiditySensor", MySqlDbType.String);
                            MySqlParameter spar7 = new MySqlParameter("@TemperatureSensor", MySqlDbType.String);
                            MySqlParameter spar8 = new MySqlParameter("@RFIDTag", MySqlDbType.String);
                            spar2.Value = SensipResources.Configurator.Accounts[index].UserName;
                            //spar3.Value = Properties.Settings.Default.cfgZones[((zone)(propertyGrid1.SelectedObject)).FullName];
                            spar3.Value = SensipResources.Configurator.Accounts[index].DisplayName;
                            spar4.Value = ((zone)(propertyGrid1.SelectedObject)).Description;
                            spar5.Value = Properties.Settings.Default.cfgRFIDReaders[((zone)(propertyGrid1.SelectedObject)).RFIDReader];
                            spar6.Value = ((zone)(propertyGrid1.SelectedObject)).HimiditySensor;
                            spar7.Value = ((zone)(propertyGrid1.SelectedObject)).TemperatureSensor;
                            spar8.Value = ((zone)(propertyGrid1.SelectedObject)).RFIDTag;
                            MySqlCommand command1 = new MySqlCommand(sqlstr1, conn);
                            command1.CommandType = CommandType.Text;
                            command1.Parameters.Add(spar2);
                            command1.Parameters.Add(spar3);
                            command1.Parameters.Add(spar4);
                            command1.Parameters.Add(spar5);
                            command1.Parameters.Add(spar6);
                            command1.Parameters.Add(spar7);
                            command1.Parameters.Add(spar8);
                            command1.ExecuteNonQuery();

                            needUpdate = true;
                            break;
                        }
                    case 1:
                        {
                            if (isInsert)
                            {
                                sqlstr1 = "INSERT INTO z_people (UserName, FullName, Description, LastName, FirstName, Age, IsFemale, RFIDTag, TemperatureSensor, BloodPressureSensor, HeartRateSensor) ";
                                sqlstr1 = sqlstr1 + "VALUES (@UserName, @FullName, @Description, @LastName, @FirstName, @Age, @IsFemale, @RFIDTag, @TemperatureSensor, @BloodPressureSensor, @HeartRateSensor)";
                            }
                            else
                            {
                                sqlstr1 = "UPDATE z_people SET UserName=@UserName, FullName=@FullName, Description=@Description, LastName=@LastName, FirstName=@FirstName, Age=@Age, IsFemale=@IsFemale, RFIDTag=@RFIDTag, ";
                                sqlstr1 = sqlstr1 + "TemperatureSensor=@TemperatureSensor, BloodPressureSensor=@BloodPressureSensor, HeartRateSensor=@HeartRateSensor";
                            } 
                            MySqlParameter qpar1 = new MySqlParameter("@UserName", MySqlDbType.String);
                            MySqlParameter qpar2 = new MySqlParameter("@FullName", MySqlDbType.String);
                            MySqlParameter qpar3 = new MySqlParameter("@Description", MySqlDbType.String);
                            MySqlParameter qpar4 = new MySqlParameter("@LastName", MySqlDbType.String);
                            MySqlParameter qpar5 = new MySqlParameter("@FirstName", MySqlDbType.String);
                            MySqlParameter qpar6 = new MySqlParameter("@Age", MySqlDbType.Int32);
                            MySqlParameter qpar7 = new MySqlParameter("@IsFemale", MySqlDbType.Text);
                            MySqlParameter qpar8 = new MySqlParameter("@RFIDTag", MySqlDbType.String);
                            MySqlParameter qpar9 = new MySqlParameter("@TemperatureSensor", MySqlDbType.String);
                            MySqlParameter qpar10 = new MySqlParameter("@BloodPressureSensor", MySqlDbType.String);
                            MySqlParameter qpar11 = new MySqlParameter("@HeartRateSensor", MySqlDbType.String);
                            
                            qpar1.Value = SensipResources.Configurator.Accounts[index].UserName;
                            qpar2.Value = SensipResources.Configurator.Accounts[index].DisplayName ;
                            qpar3.Value = ((people)(propertyGrid1.SelectedObject)).Description;
                            qpar4.Value = ((people)(propertyGrid1.SelectedObject)).LastName;
                            qpar5.Value = ((people)(propertyGrid1.SelectedObject)).FirstName;
                            qpar6.Value = ((people)(propertyGrid1.SelectedObject)).Age;
                            qpar7.Value = ((people)(propertyGrid1.SelectedObject)).IsFemale;
                            qpar8.Value = ((people)(propertyGrid1.SelectedObject)).RFIDTag;
                            qpar9.Value = ((people)(propertyGrid1.SelectedObject)).TemperatureSensor;
                            qpar10.Value = ((people)(propertyGrid1.SelectedObject)).BloodPressureSensor;
                            qpar11.Value = ((people)(propertyGrid1.SelectedObject)).HeartRateSensor;

                            MySqlCommand command1 = new MySqlCommand(sqlstr1, conn);
                            command1.CommandType = CommandType.Text;
                            command1.Parameters.Add(qpar1);
                            command1.Parameters.Add(qpar2);
                            command1.Parameters.Add(qpar3);
                            command1.Parameters.Add(qpar4);
                            command1.Parameters.Add(qpar5);
                            command1.Parameters.Add(qpar6);
                            command1.Parameters.Add(qpar7);
                            command1.Parameters.Add(qpar8);
                            command1.Parameters.Add(qpar9);
                            command1.Parameters.Add(qpar10);
                            command1.Parameters.Add(qpar11);
                            command1.ExecuteNonQuery();
                            break;
                        }
                    case 2:
                        {
                            if (isInsert)
                            {
                                sqlstr1 = "INSERT INTO z_assets (UserName, FullName, Description, RFIDTag ) ";
                                sqlstr1 = sqlstr1 + "VALUES (@UserName, @FullName, @Description, @RFIDTag)";
                            }
                            else
                            {
                                sqlstr1 = "UPDATE z_assets SET UserName=@UserName, FullName=@FullName, Description=@Description, RFIDTag=@RFIDTag";
                            }
                            MySqlParameter dpar1 = new MySqlParameter("@UserName", MySqlDbType.String);
                            MySqlParameter dpar2 = new MySqlParameter("@FullName", MySqlDbType.String);
                            MySqlParameter dpar3 = new MySqlParameter("@Description", MySqlDbType.String);
                            MySqlParameter dpar4 = new MySqlParameter("@RFIDTag", MySqlDbType.String);
                            
                            dpar1.Value = SensipResources.Configurator.Accounts[index].UserName;
                            dpar2.Value = SensipResources.Configurator.Accounts[index].DisplayName;
                            dpar3.Value = ((asset)(propertyGrid1.SelectedObject)).Description;
                            dpar4.Value = ((asset)(propertyGrid1.SelectedObject)).RFIDTag;


                            MySqlCommand command1 = new MySqlCommand(sqlstr1, conn);
                            command1.CommandType = CommandType.Text;
                            command1.Parameters.Add(dpar1);
                            command1.Parameters.Add(dpar2);
                            command1.Parameters.Add(dpar3);
                            command1.Parameters.Add(dpar4);

                            command1.ExecuteNonQuery();
                            break;
                        }
                }

                SensipResources.Configurator.Save();

                ReregisterRequired = true;
                // reinitialize stack
                if (RestartRequired) SensipResources.StackProxy.initialize();

                if (ReregisterRequired) SensipResources.Registrar.registerAccounts();

            }
            }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            IAccount account = new SensipAccount(SensipResources.Configurator.Accounts.Count);
            SensipResources.Configurator.Accounts.Add(account);
            UpdateAccountList(SensipResources.Configurator.Accounts.Count-1);
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            int index = this.listViewAccounts.SelectedItems[0].Index;
            if (index >= 0)
            {
                if (!ConnectDB()) { return; }

                IAccount account = SensipResources.Configurator.Accounts[index];
                string username = account.UserName;

                if (account.UserName.Trim() == "")
                    return;


               //--------------------------------------Save To Remote DB
                string sqlstr;

                sqlstr = "DELETE FROM z_accounts WHERE UserName=@username";
                MySqlParameter par2 = new MySqlParameter("@username", MySqlDbType.String);
                par2.Value = username;

                MySqlCommand cmd1 = new MySqlCommand(sqlstr, conn);
                cmd1.CommandType = CommandType.Text;
                cmd1.Parameters.Add(par2);
                cmd1.ExecuteNonQuery();

                //------------
                string sqlstr1;

                switch (SensipResources.Configurator.Accounts[index].ElemType)
                {
                    case 0:
                        {
                            sqlstr1 = "DELETE FROM z_zones WHERE UserName=@username";
                            MySqlParameter par = new MySqlParameter("@username", MySqlDbType.String);
                            par.Value = username;
                            MySqlCommand cmd = new MySqlCommand(sqlstr1, conn);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add(par);
                            cmd.ExecuteNonQuery();
                            break;
                        }
                    case 1:
                        {
                            sqlstr1 = "DELETE FROM z_people WHERE UserName=@username";
                            MySqlParameter par = new MySqlParameter("@username", MySqlDbType.String);
                            par.Value = username;
                            MySqlCommand cmd = new MySqlCommand(sqlstr1, conn);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add(par);
                            cmd.ExecuteNonQuery();
                            break;
                        }
                    case 2:
                        {
                            sqlstr1 = "DELETE FROM z_assets WHERE UserName=@username";
                            MySqlParameter par = new MySqlParameter("@username", MySqlDbType.String);
                            par.Value = username;
                            MySqlCommand cmd = new MySqlCommand(sqlstr1, conn);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add(par);
                            cmd.ExecuteNonQuery();
                            break;
                        }
                    default:
                        break;
                }
                


                //--------------------------------------Save To Local
                account.HostName = SensipResources.Configurator.Accounts[0].HostName;

                account.DisplayName = "";
                account.Id = "";
                account.UserName = "";
                account.Password = "";
                account.Description = "";
                account.Picture = "";
                account.AccountName = "";
                account.ElemType = -1;

                if (account.Picture.Trim() != "")
                {
                    string FileNewPath = strCurrentPath + "\\data\\" + account.UserName + System.IO.Path.GetExtension(account.Picture);
                    if (File.Exists(FileNewPath))
                    {
                        System.IO.File.Delete(FileNewPath);
                    }
                }
                account.Picture = "";

                SensipResources.Configurator.Save();


                ReregisterRequired = true;
                // reinitialize stack
                if (RestartRequired) SensipResources.StackProxy.initialize();

                if (ReregisterRequired) SensipResources.Registrar.registerAccounts();
                UpdateAccountList(0);

            }
        }



        private void dataPropertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int index = listViewAccounts.SelectedItems[0].Index;
            IAccount acc = SensipResources.Configurator.Accounts[index];
            string username = acc.UserName;
            ElemType type = (ElemType)(acc.ElemType);

            rtDataManager.SaveRTDataRow(username, type);
            string selectedIteminfo = rtDataManager.GenerateComplexMessage(username, type);
            PublishComplexStatus(acc.Index, selectedIteminfo);
        }

        private void toolStripButtonCall_Click(object sender, EventArgs e)
        {
            if (toolStripComboDial.Text.Length > 0)
            {
                MessageBox.Show("Middleware is calling ");
                SensipResources.CallManager.createOutboundCall(toolStripComboDial.Text);
            }
        }

        private void toolStripButtonRelease_Click(object sender, EventArgs e)
        {
            if (listViewCallLines.Items.Count > 0)
            {
                ListViewItem lvi = listViewCallLines.SelectedItems[0];
                SensipResources.CallManager.onUserRelease(((CStateMachine)lvi.Tag).Session);
            }
        }

        private void dataPropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {


        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            (new SettingsForm(this.SensipResources, false)).ShowDialog();
            RefreshForm();
        }

        private void toolStripReaders_Click(object sender, EventArgs e)
        {
                RFIDFrm.ShowDialog();
        }

        private void devicesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        
        //publish Information
        private void timer1_Tick(object sender, EventArgs e)
        {
            //传播状态
            try
            {
                for (int index = 1; index < SensipResources.Configurator.Accounts.Count; index++)
                {
                    IAccount acc = SensipResources.Configurator.Accounts[index];
                    string username = acc.UserName;
                    if (acc.UserName.Length == 0)
                    {
                        return;
                    }

                    ElemType type = (ElemType)(acc.ElemType);
                    string selectedIteminfo = rtDataManager.GenerateComplexMessage(username, type);
                    PublishComplexStatus(index, selectedIteminfo);
                }

                //更新选中的account的图形显示
                if (listViewAccounts.SelectedItems.Count > 0)
                {
                    int index = listViewAccounts.SelectedItems[0].Index;
                    IAccount acc = SensipResources.Configurator.Accounts[index];
                    if (acc == null)
                    {
                        // error!!!
                        return;
                    }
                    updateRTShows(acc.UserName, (ElemType)(acc.ElemType), acc.DisplayName);
                }
            }

            catch (Exception es)
            {
                return;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string
            if (listViewAccounts.SelectedItems.Count > 0)
            {
                if (!ConnectDB()) { return; }

                int index = listViewAccounts.SelectedItems[0].Index;
                IAccount acc = SensipResources.Configurator.Accounts[index];
                SpeakStr = rtDataManager.GetSpeakString(acc.UserName, (ElemType)(acc.ElemType));
                VM.Speak(SpeakStr);
            }
        }

        delegate void DEnablelabelAlarm(string temp, string motion);

        public void CEPAlarm(double temp, string motion)
        {
            //SensipResources.CallManager.createOutboundCall("55");
            //CEPTimer.Enabled = true;
            string t = temp.ToString();

            if (InvokeRequired)
                this.BeginInvoke(new DEnablelabelAlarm(this.EnablelabelAlarm), new object[] { t, motion });
            else
                EnablelabelAlarm(t, motion);
        }

        private void EnablelabelAlarm(string temp, string motion)
        {
            labelAlarm.Enabled = true;
            labelTemp.Text = temp;
            labelMotion.Text = motion;
            VM.Speak("Object is moving and the temperature is too high!");
            labelAlarm.Enabled = false;
            labelTemp.Text = "";
            labelTemp.Text = "";
        }


        private void CEPTimer_Tick(object sender, EventArgs e)
        {
            //labelAlarm.Enabled = false;
            //CEPTimer.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

#region Chart
        delegate void DAddValue(string UserName, double temp);


        GraphPane myPane;
        LineItem myCurve;
        double index =0;
        bool isStarted = false;

        private void setupChart()
        {
            myPane = zg1.GraphPane;
            // Set the titles and axis labels
            myPane.Title.Text = "Temperature History";
            myPane.XAxis.Title.Text = "Time";
            myPane.YAxis.Title.Text = "Temperature";

            PointPairList list = new PointPairList();
            list.Add(0, 0);
            myCurve = myPane.AddCurve("Temperature", list, Color.Blue, SymbolType.Circle);
            myCurve.Line.Fill = new Fill(Color.White, Color.Red, 45F);
            myCurve.Symbol.Fill = new Fill(Color.White);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            myCurve.Clear();
            isStarted = true;
        }

        public void updateChart(string UserName, double temp)
        {
            if (InvokeRequired)
                this.BeginInvoke(new DAddValue(this.AddValue), new object[] { UserName, temp });
            else
                AddValue(UserName, temp);
        }

        private void AddValue(string UserName, double temp)
        {
            if (listViewAccounts.SelectedItems.Count > 0)
            {
                string uName = listViewAccounts.SelectedItems[0].SubItems[2].Text;
                if (uName != UserName)
                    return;
            }

            if (isStarted == false)
                return;

            if (myCurve == null)
                return;
            try
            {
                myCurve.AddPoint(index, temp);
                index = index + 1;
                zg1.AxisChange();
                zg1.Refresh();
            }
            catch (Exception ee)
            {
                return;
            }

        }
#endregion

        private void button3_Click(object sender, EventArgs e)
        {
            isStarted = false;
            myCurve.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }



}


 