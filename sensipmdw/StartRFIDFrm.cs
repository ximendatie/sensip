using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Sensip
{
    public partial class StartRFIDFrm : Form
    {
        MainFrm frm;
        public StartRFIDFrm(MainFrm f)
        {
            InitializeComponent();
            frm = f;

            if (!LoadRTData())
                return;
            if (!LoadMapping())
                return;
            UpdateListView();
        }

        //Tag---> UserName, ElemType(people, asset)
        public static Dictionary<string, string[]> ObjMapping = new Dictionary<string, string[]>();
        //ReaderIP---> UserName
        public static Dictionary<string, string> ZoneMapping = new Dictionary<string, string>();
        
        

        DataSet DCDS = new DataSet();

        private bool LoadMapping()
        {
            DataTable tblObjMapping, tblZoneMapping;
            DataSet DS = new DataSet() ;

            string strCurrentPath = System.IO.Directory.GetCurrentDirectory().ToString();
            string path = strCurrentPath + "\\data\\" + Properties.Settings.Default.DeviceCfgFile;
            if (!File.Exists(path))
            {
                MessageBox.Show("Cannot Find the Device Config File!", "Error");
                return false;
            }
            try
            {
                DS.ReadXml(path);
                tblObjMapping = DS.Tables["ObjMapping"];
                tblZoneMapping = DS.Tables["ZoneMapping"];

                ObjMapping.Clear() ;
                ZoneMapping.Clear() ;

                int i;
                for (i = 0; i < tblObjMapping.Rows.Count; i++)
                {
                    DataRow drow = tblObjMapping.Rows[i];
                    ObjMapping.Add(drow["Tag"].ToString(), new string[] { drow["UserName"].ToString(), drow["ElemType"].ToString() });
                }

                for (i = 0; i < tblZoneMapping.Rows.Count; i++)
                {
                    DataRow drow = tblZoneMapping.Rows[i];
                    ZoneMapping.Add(drow["Reader"].ToString(), drow["UserName"].ToString());
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }

        
            string tbName = "ReaderCfg";
        DataTable table;

        private bool LoadRTData()
        {
            string strCurrentPath = System.IO.Directory.GetCurrentDirectory().ToString();
            string path = strCurrentPath + "\\data\\" + Properties.Settings.Default.DeviceCfgFile;

            if (!File.Exists(path))
            {
                MessageBox.Show("Cannot Find the Device Config File!", "Error");
                return false;
            }
            try
            {
                DCDS.ReadXml(path);

                table = DCDS.Tables[tbName];
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }

        public void WriteRTDataRow(string fieldname, string value)
        {
            string tbName = "ReaderCfg";

            DataTable table = DCDS.Tables[tbName];
            /* DataRow drow = table.Rows.Find(fieldname);
            if (drow == null)
            {
                drow = table.NewRow();
                drow["UserName"] = username;
                table.BeginLoadData();
                table.LoadDataRow(drow.ItemArray, true);
                table.EndLoadData();
                table.AcceptChanges();
            }

            drow = table.Rows.Find(username);
            drow[fieldname] = value;
            drow.AcceptChanges();
            */
        }
        
        private void UpdateListView()
        {
            listView1.Items.Clear();
            
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow drow = table.Rows[i];
                string running ;
                
                if (Int16.Parse(drow["Running"].ToString()) > 0)
                    running = "Running";
                else
                    running = "Stopped";
                ListViewItem item = new ListViewItem(new string[] { drow["ReaderIP"].ToString(), drow["Type"].ToString(), running });
                listView1.Items.Add(item);
            }
        }

        private void StartRFIDFrm_Load(object sender, EventArgs e)
        {
            
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        XtiveDP xtiveDP = null;
        Crossbow crossbowDP = null;

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].SubItems[2].Text == "Running")
                    return ;

                //start 
                DataRow drow = table.Rows[listView1.SelectedItems[0].Index];
                bool status = false;
                switch (drow["type"].ToString())
                {
                    case "xtive":
                        if (xtiveDP == null)
                            xtiveDP = new XtiveDP(drow["ReaderIP"].ToString());
                        xtiveDP.StartProcessing();
                        status = xtiveDP.CheckStatus();
                        break;
                    case "alien":
                        break;
                    case "Crossbow":
                        if (crossbowDP == null)
                            crossbowDP = new Crossbow(drow["ReaderIP"].ToString());
                        crossbowDP.StartProcessing();
                        status = crossbowDP.CheckStatus();
                        break; 
                    default :
                        break;
                }
                //change the rt database
                
                drow["Running"] = status;
                frm.SetUpdateTimer(status);

                UpdateListView();
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].SubItems[2].Text == "Stopped")
                    return;

                //change the rt database
                DataRow drow = table.Rows[listView1.SelectedItems[0].Index];
                if (xtiveDP != null)
                {
                    xtiveDP.StopProcessing();

                }
                drow["Running"] = 0;
                frm.SetUpdateTimer(false);

                UpdateListView();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void StartRFIDFrm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        public void StopProcessing()
        {
            if (xtiveDP != null)
            xtiveDP.StopProcessing();
        }

        private void StartRFIDFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }



    }
}