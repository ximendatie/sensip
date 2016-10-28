using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sensip.Common.CallControl;
using Sensip.Common; 

namespace Sensip
{
    public partial class Elements : Form
    {
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

        public Elements(SensipResources resources)
        {
            InitializeComponent();
            _resources = resources;
        }

        //do not need to manage the middleware account

        private void updateAccountList()
        {
            int size = SensipResources.Configurator.Accounts.Count;
            comboBoxAccounts.Items.Clear();
            for (int i = 1; i < size; i++) //from 1, since the 0 is for the middleware
            {
                IAccount acc = SensipResources.Configurator.Accounts[i];

                if (acc.AccountName.Length == 0)
                {
                    comboBoxAccounts.Items.Add("No Elements");
                }
                else
                {
                    comboBoxAccounts.Items.Add(acc.AccountName);
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void clearAll()
        {
            textBoxAccountName.Text = "";
            textBoxDisplayName.Text = "";
            textBoxUsername.Text = "";
            textBoxPassword.Text = "";
        }


        private void comboBoxAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBoxAccounts.SelectedIndex;

            //from 1, since the 0 is for the middleware
            IAccount acc = SensipResources.Configurator.Accounts[index+1];

            if (acc == null)
            {
                clearAll();
                // error!!!
                return;
            }

            textBoxAccountName.Text = acc.AccountName;
            textBoxDisplayName.Text = acc.DisplayName;
            textBoxUsername.Text = acc.UserName;
            textBoxPassword.Text = acc.Password;

            
        }



        private void buttonApply_Click(object sender, EventArgs e)
        {
            int index = this.comboBoxAccounts.SelectedIndex;
            if (index >= 0)
            {
                IAccount account = SensipResources.Configurator.Accounts[index];

                account.HostName = textBoxRegistrarAddress.Text;
                account.AccountName = textBoxAccountName.Text;
                account.DisplayName = textBoxDisplayName.Text;
                account.Id = textBoxUsername.Text;
                account.UserName = textBoxUsername.Text;
                account.Password = textBoxPassword.Text;

                account.ProxyAddress = SensipResources.Configurator.Accounts[0].ProxyAddress;
                account.DomainName = SensipResources.Configurator.Accounts[0].DomainName;
                account.TransportMode = SensipResources.Configurator.Accounts[0].TransportMode;
            }
            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            buttonApply_Click(sender, e);

            SensipResources.Configurator.Save();

            // reinitialize stack
            if (RestartRequired) SensipResources.StackProxy.initialize();

            if (ReregisterRequired) SensipResources.Registrar.registerAccounts();

            Close();
        }

        private void Elements_Load(object sender, EventArgs e)
        {
            // Continued
            updateAccountList();

            // set stack flags
            ReregisterRequired = false;
            RestartRequired = false;
        }





    }
}