/* 
 * Copyright (C) 2008 Sasa Coh <sasacoh@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
 * 
 * WaveLib library sources http://www.codeproject.com/KB/graphics/AudioLib.aspx
 * 
 * Visit SensipSDK page at http://voipengine.googlepages.com/
 * 
 * Visit Sensip's home page at http://Sensipphone.googlepages.com/ 
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Runtime.InteropServices;
using System.Media;
using Sensip.Common;
using Sensip.Common.CallControl;
using Sensip.Sip;


namespace Sensip
{
    /// <summary>
    /// ConcreteFactory 
    /// Implementation of AbstractFactory. 
    /// </summary>
    public class SensipResources : AbstractFactory
    {
        MainFrm _form; // reference to MainForm to provide timer context
        IMediaProxyInterface _mediaProxy = new CMediaPlayerProxy();
        ICallLogInterface _callLogger = new CCallLog();
        pjsipStackProxy _stackProxy = pjsipStackProxy.Instance;
        SensipConfigurator _config = new SensipConfigurator();

        #region Constructor
        public SensipResources(MainFrm mf)
        {
            _form = mf;

            // initialize sip struct at startup
            SipConfigStruct.Instance.stunServer = this.Configurator.StunServerAddress;
            SipConfigStruct.Instance.publishEnabled = this.Configurator.PublishEnabled;
            SipConfigStruct.Instance.expires = this.Configurator.Expires;
            SipConfigStruct.Instance.VADEnabled = this.Configurator.VADEnabled;
            SipConfigStruct.Instance.ECTail = this.Configurator.ECTail;
            SipConfigStruct.Instance.nameServer = this.Configurator.NameServer;

            // initialize modules
            _callManager.StackProxy = _stackProxy;
            _callManager.Config = _config;
            _callManager.Factory = this;
            _callManager.MediaProxy = _mediaProxy;
            _stackProxy.Config = _config;
            _registrar.Config = _config;
            _messenger.Config = _config;

            // do not save account state
            /*for (int i = 0; i < Properties.Settings.Default.ActualAccountNum; i++)
            {
                Properties.Settings.Default.cfgSipAccountState[i] = "0";
                
            }*/
            for (int i = 0; i < Properties.Settings.Default.cfgSipAccountUsername.Count; i++)
            {
                Properties.Settings.Default.cfgSipAccountState[i] = "0";

            }

        }
        #endregion Constructor

        #region AbstractFactory methods
        public ITimer createTimer()
        {
            return new GUITimer(_form);
        }

        public IStateMachine createStateMachine()
        {
            // TODO: check max number of calls
            return new CStateMachine();
        }

        #endregion

        #region Other Resources
        public pjsipStackProxy StackProxy
        {
            get { return _stackProxy; }
            set { _stackProxy = value; }
        }

        public SensipConfigurator Configurator
        {
            get { return _config; }
            set { }
        }

        // getters
        public IMediaProxyInterface MediaProxy
        {
            get { return _mediaProxy; }
            set { }
        }

        public ICallLogInterface CallLogger
        {
            get { return _callLogger; }
            set { }
        }

        private IRegistrar _registrar = pjsipRegistrar.Instance;
        public IRegistrar Registrar
        {
            get { return _registrar; }
        }

        private IPresenceAndMessaging _messenger = pjsipPresenceAndMessaging.Instance;
        public IPresenceAndMessaging Messenger
        {
            get { return _messenger; }
        }

        private CCallManager _callManager = CCallManager.Instance;
        public CCallManager CallManager
        {
            get { return CCallManager.Instance; }
        }
        #endregion
    }

    //#region Concrete implementations

    public class GUITimer : ITimer
    {
        Timer _guiTimer;
        MainFrm _form;


        public GUITimer(MainFrm mf)
        {
            _form = mf;
            _guiTimer = new Timer();
            if (this.Interval > 0) _guiTimer.Interval = this.Interval;
            _guiTimer.Interval = 100;
            _guiTimer.Enabled = true;
            _guiTimer.Elapsed += new ElapsedEventHandler(_guiTimer_Tick);
        }

        void _guiTimer_Tick(object sender, EventArgs e)
        {
            _guiTimer.Stop();
            //_elapsed(sender, e);
            // Synchronize thread with GUI because SIP stack works with GUI thread only
            if ((_form.IsDisposed) || (_form.Disposing) || (!_form.IsInitialized))
            {
                return;
            }
            _form.Invoke(_elapsed, new object[] { sender, e });
        }

        public bool Start()
        {
            _guiTimer.Start();
            return true;
        }

        public bool Stop()
        {
            _guiTimer.Stop();
            return true;
        }

        private int _interval;
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; _guiTimer.Interval = value; }
        }

        private TimerExpiredCallback _elapsed;
        public TimerExpiredCallback Elapsed
        {
            set
            {
                _elapsed = value;
            }
        }
    }


    // Accounts
    public class SensipAccount1 : IAccount
    {
        private int _index = -1;
        private int _accountIdentification = -1;

        /// <summary>
        /// Temp storage!
        /// The account index assigned by voip stack
        /// </summary>
        public int Index
        {
            get
            {
                int value;
                if (Int32.TryParse(Properties.Settings.Default.cfgSipAccountIndex[_index], out value))
                {
                    return value;
                }
                return -1;
            }
            set { Properties.Settings.Default.cfgSipAccountIndex[_index] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">the account identification used by configuration (values 0..4)</param>
        public SensipAccount1(int index)
        {
            _index = index;
            /*Properties.Settings.Default.cfgSipAccountNames.Add("");
            Properties.Settings.Default.cfgSipAccountIndex.Add("-1");
            Properties.Settings.Default.cfgSipAccountAddresses.Add("");
            Properties.Settings.Default.cfgSipAccountIds.Add("") ;
            Properties.Settings.Default.cfgSipAccountUsername.Add("") ;
            Properties.Settings.Default.cfgSipAccountPassword.Add("") ;
            Properties.Settings.Default.cfgSipAccountDescription.Add("") ;
            Properties.Settings.Default.cfgSipAccountPicture.Add("") ;
            Properties.Settings.Default.cfgSipAccountElemtype.Add("100") ;
            Properties.Settings.Default.cfgSipAccountDisplayName.Add("") ;
            Properties.Settings.Default.cfgSipAccountDomains.Add("*") ;
            Properties.Settings.Default.cfgSipAccountState.Add("0") ;
            Properties.Settings.Default.cfgSipAccountProxyAddresses.Add("") ;
            Properties.Settings.Default.cfgSipAccountTransport.Add("ETransportMode.TM_UDP");*/
        }



        public string AccountName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountNames[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountNames[_index] = value;
            }
        }

        public string HostName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountAddresses[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountAddresses[_index] = value;
            }
        }

        public string Id
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountIds[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountIds[_index] = value;
            }
        }

        public string UserName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountUsername[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountUsername[_index] = value;
            }
        }

        public string Password
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountPassword[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountPassword[_index] = value;
            }
        }

        public string Description
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountDescription[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountDescription[_index] = value;
            }
        }

        public string Picture
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountPicture[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountPicture[_index] = value;
            }
        }
        
        public int ElemType
        {
            get
            {
                int value;
                if (Int32.TryParse(Properties.Settings.Default.cfgSipAccountElemtype[_index], out value))
                {
                    return value;
                }
                return -1;
            }
            set { Properties.Settings.Default.cfgSipAccountElemtype[_index] = value.ToString(); }
        }

        public string DisplayName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountDisplayName[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountDisplayName[_index] = value;
            }
        }

        public string DomainName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountDomains[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountDomains[_index] = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
            set
            {
                ;
            }
        }

        public int RegState
        {
            get
            {
                int value;
                if (Int32.TryParse(Properties.Settings.Default.cfgSipAccountState[_index], out value))
                {
                    return value;
                }
                return 0;
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountState[_index] = value.ToString();
            }
        }

        public string ProxyAddress
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountProxyAddresses[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountProxyAddresses[_index] = value;
            }
        }

        public ETransportMode TransportMode
        {
            get
            {
                int value;
                if (Int32.TryParse(Properties.Settings.Default.cfgSipAccountTransport[_index], out value))
                {
                    return (ETransportMode)value;
                }
                return (ETransportMode.TM_UDP); // default
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountTransport[_index] = ((int)value).ToString();
            }
        }


    }

    public class SensipAccount : IAccount
    {
        private int _index = -1;
        private int _accountIdentification = -1;

        /// <summary>
        /// Temp storage!
        /// The account index assigned by voip stack
        /// </summary>
        public int Index
        {
            get
            {
                int value;
                if (Int32.TryParse(Properties.Settings.Default.cfgSipAccountIndex[_index], out value))
                {
                    return value;
                }
                return -1;
            }
            set { Properties.Settings.Default.cfgSipAccountIndex[_index] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">the account identification used by configuration (values 0..4)</param>
        public SensipAccount(int index)
        {
            _index = index;

            while (_index >= Properties.Settings.Default.cfgSipAccountUsername.Count)
            {
                Properties.Settings.Default.cfgSipAccountNames.Add("");
                Properties.Settings.Default.cfgSipAccountIndex.Add("-1");
                Properties.Settings.Default.cfgSipAccountAddresses.Add("");
                Properties.Settings.Default.cfgSipAccountIds.Add("");
                Properties.Settings.Default.cfgSipAccountUsername.Add("");
                Properties.Settings.Default.cfgSipAccountPassword.Add("");
                Properties.Settings.Default.cfgSipAccountDescription.Add("");
                Properties.Settings.Default.cfgSipAccountPicture.Add("");
                Properties.Settings.Default.cfgSipAccountElemtype.Add("100");
                Properties.Settings.Default.cfgSipAccountDisplayName.Add("");
                Properties.Settings.Default.cfgSipAccountDomains.Add("*");
                Properties.Settings.Default.cfgSipAccountState.Add("0");
                Properties.Settings.Default.cfgSipAccountProxyAddresses.Add("");
                Properties.Settings.Default.cfgSipAccountTransport.Add("ETransportMode.TM_UDP");
            }


        }



        public string AccountName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountNames[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountNames[_index] = value;
            }
        }

        public string HostName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountAddresses[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountAddresses[_index] = value;
            }
        }

        public string Id
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountIds[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountIds[_index] = value;
            }
        }

        public string UserName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountUsername[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountUsername[_index] = value;
            }
        }

        public string Password
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountPassword[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountPassword[_index] = value;
            }
        }

        public string Description
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountDescription[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountDescription[_index] = value;
            }
        }

        public string Picture
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountPicture[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountPicture[_index] = value;
            }
        }

        public int ElemType
        {
            get
            {
                int value;
                if (Int32.TryParse(Properties.Settings.Default.cfgSipAccountElemtype[_index], out value))
                {
                    return value;
                }
                return -1;
            }
            set { Properties.Settings.Default.cfgSipAccountElemtype[_index] = value.ToString(); }
        }

        public string DisplayName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountDisplayName[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountDisplayName[_index] = value;
            }
        }

        public string DomainName
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountDomains[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountDomains[_index] = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
            set
            {
                ;
            }
        }

        public int RegState
        {
            get
            {
                int value;
                if (Int32.TryParse(Properties.Settings.Default.cfgSipAccountState[_index], out value))
                {
                    return value;
                }
                return 0;
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountState[_index] = value.ToString();
            }
        }

        public string ProxyAddress
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountProxyAddresses[_index];
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountProxyAddresses[_index] = value;
            }
        }

        public ETransportMode TransportMode
        {
            get
            {
                int value;
                if (Int32.TryParse(Properties.Settings.Default.cfgSipAccountTransport[_index], out value))
                {
                    return (ETransportMode)value;
                }
                return (ETransportMode.TM_UDP); // default
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountTransport[_index] = ((int)value).ToString();
            }
        }


    }



    /// <summary>
    /// 
    /// </summary>
    public class SensipConfigurator : IConfiguratorInterface
    {
        List<IAccount> accList = null ; 

        public SensipConfigurator()
        {
             accList= new List<IAccount>();
             for (int i = 0; i < Properties.Settings.Default.cfgSipAccountUsername.Count; i++)
             {
                 IAccount account = new SensipAccount(i);
                 accList.Add(account);

                 account.HostName = Properties.Settings.Default.cfgSipAccountAddresses[i];
                 account.ProxyAddress = Properties.Settings.Default.cfgSipAccountProxyAddresses[i];
                 account.AccountName = Properties.Settings.Default.cfgSipAccountNames[i];
                 account.DisplayName = Properties.Settings.Default.cfgSipAccountDisplayName[i];
                 account.Id = Properties.Settings.Default.cfgSipAccountIds[i];
                 account.UserName = Properties.Settings.Default.cfgSipAccountUsername[i];
                 account.Password = Properties.Settings.Default.cfgSipAccountPassword[i];
                 account.DomainName = Properties.Settings.Default.cfgSipAccountDomains[i];
                 account.TransportMode = 0;
                 account.Picture = Properties.Settings.Default.cfgSipAccountPicture[i];
                 account.RegState = Int32.Parse(Properties.Settings.Default.cfgSipAccountState[i]);
                 account.Description = Properties.Settings.Default.cfgSipAccountDescription[i];
                 account.Index = Int32.Parse(Properties.Settings.Default.cfgSipAccountIndex[i]);
                 account.ElemType = Int32.Parse(Properties.Settings.Default.cfgSipAccountElemtype[i]);
             }
        }

        public bool IsNull { get { return false; } }

        public bool CFUFlag
        {
            get { return Properties.Settings.Default.cfgCFUFlag; }
            set { Properties.Settings.Default.cfgCFUFlag = value; }
        }
        public string CFUNumber
        {
            get { return Properties.Settings.Default.cfgCFUNumber; }
            set { Properties.Settings.Default.cfgCFUNumber = value; }
        }
        public bool CFNRFlag
        {
            get { return Properties.Settings.Default.cfgCFNRFlag; }
            set { Properties.Settings.Default.cfgCFNRFlag = value; }
        }
        public string CFNRNumber
        {
            get { return Properties.Settings.Default.cfgCFNRNumber; }
            set { Properties.Settings.Default.cfgCFNRNumber = value; }
        }
        public bool DNDFlag
        {
            get { return Properties.Settings.Default.cfgDNDFlag; }
            set { Properties.Settings.Default.cfgDNDFlag = value; }
        }
        public bool AAFlag
        {
            get { return Properties.Settings.Default.cfgAAFlag; }
            set { Properties.Settings.Default.cfgAAFlag = value; }
        }

        public bool CFBFlag
        {
            get { return Properties.Settings.Default.cfgCFBFlag; }
            set { Properties.Settings.Default.cfgCFBFlag = value; }
        }

        public string CFBNumber
        {
            get { return Properties.Settings.Default.cfgCFBNumber; }
            set { Properties.Settings.Default.cfgCFBNumber = value; }
        }

        public int SIPPort
        {
            get { return Properties.Settings.Default.cfgSipPort; }
            set { Properties.Settings.Default.cfgSipPort = value; }
        }

        public bool PublishEnabled
        {
            get
            {
                SipConfigStruct.Instance.publishEnabled = Properties.Settings.Default.cfgSipPublishEnabled;
                return Properties.Settings.Default.cfgSipPublishEnabled;
            }
            set
            {
                SipConfigStruct.Instance.publishEnabled = value;
                Properties.Settings.Default.cfgSipPublishEnabled = value;
            }
        }

        public string StunServerAddress
        {
            get
            {
                SipConfigStruct.Instance.stunServer = Properties.Settings.Default.cfgStunServerAddress;
                return Properties.Settings.Default.cfgStunServerAddress;
            }
            set
            {
                Properties.Settings.Default.cfgStunServerAddress = value;
                SipConfigStruct.Instance.stunServer = value;
            }
        }

        public EDtmfMode DtmfMode
        {
            get
            {
                return (EDtmfMode)Properties.Settings.Default.cfgDtmfMode;
            }
            set
            {
                Properties.Settings.Default.cfgDtmfMode = (int)value;
            }
        }

        public int Expires
        {
            get
            {
                SipConfigStruct.Instance.expires = Properties.Settings.Default.cfgRegistrationTimeout;
                return Properties.Settings.Default.cfgRegistrationTimeout;
            }
            set
            {
                Properties.Settings.Default.cfgRegistrationTimeout = value;
                SipConfigStruct.Instance.expires = value;
            }
        }

        public int ECTail
        {
            get
            {
                SipConfigStruct.Instance.ECTail = Properties.Settings.Default.cfgECTail;
                return Properties.Settings.Default.cfgECTail;
            }
            set
            {
                Properties.Settings.Default.cfgECTail = value;
                SipConfigStruct.Instance.ECTail = value;
            }
        }

        public bool VADEnabled
        {
            get
            {
                SipConfigStruct.Instance.VADEnabled = Properties.Settings.Default.cfgVAD;
                return Properties.Settings.Default.cfgVAD;
            }
            set
            {
                Properties.Settings.Default.cfgVAD = value;
                SipConfigStruct.Instance.VADEnabled = value;
            }
        }


        public string NameServer
        {
            get
            {
                SipConfigStruct.Instance.nameServer = Properties.Settings.Default.cfgNameServer;
                return Properties.Settings.Default.cfgNameServer;
            }
            set
            {
                Properties.Settings.Default.cfgNameServer = value;
                SipConfigStruct.Instance.nameServer = value;
            }
        }

        /// <summary>
        /// The position of default account in account list. Does NOT mean same as DefaultAccountIndex
        /// </summary>
        public int DefaultAccountIndex
        {
            get
            {
                return Properties.Settings.Default.cfgSipAccountDefault;
            }
            set
            {
                Properties.Settings.Default.cfgSipAccountDefault = value;
            }
        }

       

        public List<IAccount> Accounts
        {
            get
            {
                
                
                /*
                    for (int i = 0; i < Properties.Settings.Default.cfgSipAccountUsername.Count; i++)
                    {
                        IAccount item = new SensipAccount(i);
                        accList.Add(item);
                    }
                */
                return accList;

            }
        }

        public void Save()
        {
            // save properties
            Properties.Settings.Default.Save();
            
        }

        public List<string> CodecList
        {
            get
            {
                List<string> codecList = new List<string>();
                foreach (string item in Properties.Settings.Default.cfgCodecList)
                {
                    codecList.Add(item);
                }
                return codecList;
            }
            set
            {
                Properties.Settings.Default.cfgCodecList.Clear();
                List<string> cl = value;
                foreach (string item in cl)
                {
                    Properties.Settings.Default.cfgCodecList.Add(item);
                }
            }
        }
    }


    //////////////////////////////////////////////////////
    // Media proxy
    // internal class
    public class CMediaPlayerProxy : IMediaProxyInterface
    {
        SoundPlayer player = new SoundPlayer();

        #region Methods

        public int playTone(ETones toneId)
        {
            string fname;

            switch (toneId)
            {
                case ETones.EToneDial:
                    fname = "Sounds/dial.wav";
                    break;
                case ETones.EToneCongestion:
                    fname = "Sounds/congestion.wav";
                    break;
                case ETones.EToneRingback:
                    fname = "Sounds/ringback.wav";
                    break;
                case ETones.EToneRing:
                    fname = "Sounds/ring.wav";
                    break;
                default:
                    fname = "Sounds/ring.wav";
                    break;
            }

            player.SoundLocation = fname;
            player.Load();
            player.PlayLooping();

            return 1;
        }

        public int stopTone()
        {
            player.Stop();
            return 1;
        }

        #endregion

    }

   // #endregion Concrete Implementations

}
