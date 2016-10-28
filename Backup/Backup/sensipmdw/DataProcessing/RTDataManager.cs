using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Sensip
{
    
    public class RTDataManager
    {

        public RTDataManager()
        {
        }

        DataSet RTDS = new DataSet();

        string RTDataFile;

        public bool LoadRTData()
        {
            string strCurrentPath = System.IO.Directory.GetCurrentDirectory().ToString();
            string path = strCurrentPath + "\\data\\" + Properties.Settings.Default.RTDataFile;
            if (!File.Exists(path))
            {
                MessageBox.Show("Cannot Find the Real-time Data File!", "Error");
                return false;
            }
            try
            {
                RTDS.ReadXml(path);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }


        public string GetFieldInfo(string username, string fieldname)
        {
            string tbName = "";
            for (ElemType i = ElemType.etZone; i < ElemType.etOther; i++)
            {
                switch (i)
                {
                    case ElemType.etZone:
                        tbName = "ZoneInfo";
                        break;
                    case ElemType.etPeople:
                        tbName = "PeopleInfo";
                        break;
                    case ElemType.etAsset:
                        tbName = "AssetInfo";
                        break;
                }
                DataTable currTable = RTDS.Tables[tbName];
                DataRow drow = currTable.Rows.Find(username);
                if (drow != null)
                {
                    try
                    {
                        return drow[fieldname].ToString();
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
            return "";
        }

        public string GetFieldInfo(string username, ElemType dtype, string fieldname)
        {
            string tbName = "";
            switch (dtype)
            {
                case ElemType.etZone:
                    tbName = "ZoneInfo";
                    break;
                case ElemType.etPeople:
                    tbName = "PeopleInfo";
                    break;
                case ElemType.etAsset:
                    tbName = "AssetInfo";
                    break;
            }
            DataTable currTable = RTDS.Tables[tbName];
            DataRow drow = currTable.Rows.Find(username);
            if (drow != null)
            {
                try
                {
                    return drow[fieldname].ToString();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
            return "";
        }

        public DataRow LoadRTDataRow(string username, ElemType dtype)
        {
            string tbName = "";
            switch (dtype)
            {
                case ElemType.etZone:
                    tbName = "ZoneInfo";
                    break;
                case ElemType.etPeople:
                    tbName = "PeopleInfo";
                    break;
                case ElemType.etAsset:
                    tbName = "AssetInfo";
                    break;
            }
            DataTable currTable = RTDS.Tables[tbName];
            DataRow drow = currTable.Rows.Find(username);
            if (drow == null)
            {
                drow = currTable.NewRow();
                drow["UserName"] = username;
                currTable.BeginLoadData();
                currTable.LoadDataRow(drow.ItemArray, true);
                currTable.EndLoadData();
                currTable.AcceptChanges();
            }
            else
            {
            }
            return (currTable.Rows.Find(username));
        }

        public void SaveRTDataRow(string username, ElemType dtype)
        {
            string tbName = "";
            switch (dtype)
            {
                case ElemType.etZone:
                    tbName = "ZoneInfo";
                    break;
                case ElemType.etPeople:
                    tbName = "PeopleInfo";
                    break;
                case ElemType.etAsset:
                    tbName = "AssetInfo";
                    break;
                default:
                    return;
            }
            DataTable currTable = RTDS.Tables[tbName];

            //the row which will be changed.
            DataRow drow = currTable.Rows.Find(username);
            drow.AcceptChanges();

            //string strCurrentPath = System.IO.Directory.GetCurrentDirectory().ToString();
            //string path = strCurrentPath + "\\data\\" + Properties.Settings.Default.RTDataFile;
            //RTDS.WriteXml(path, XmlWriteMode.WriteSchema);
            //send message
        }

        public string GetSpeakString(string username, ElemType dtype)
        {
            string tbName = "";
            string ElemTypeName = "";
            switch (dtype)
            {
                case ElemType.etZone:
                    tbName = "ZoneInfo";
                    ElemTypeName = "etZone";
                    break;
                case ElemType.etPeople:
                    tbName = "PeopleInfo";
                    ElemTypeName = "etPeople";
                    break;
                case ElemType.etAsset:
                    tbName = "AssetInfo";
                    ElemTypeName = "etAsset";
                    break;
                default:
                    return "";
            }
            DataTable currTable = RTDS.Tables[tbName];
            DataRow drow = currTable.Rows.Find(username);
            if (drow == null)
            {
                return "";
            }
            else
            {
                string xmlinfo = "";
                foreach (DataColumn column in currTable.Columns)
                {
                    if (drow[column].ToString() != "")
                        xmlinfo = xmlinfo + "the value of "+ column.Caption + " is " + drow[column] + ",  ";

                }
                return xmlinfo;
            }
        }

        public string GenerateComplexMessage(string username, ElemType dtype)
        {
            string tbName = "";
            string ElemTypeName = "";
            switch (dtype)
            {
                case ElemType.etZone:
                    tbName = "ZoneInfo";
                    ElemTypeName = "etZone";
                    break;
                case ElemType.etPeople:
                    tbName = "PeopleInfo";
                    ElemTypeName = "etPeople";
                    break;
                case ElemType.etAsset:
                    tbName = "AssetInfo";
                    ElemTypeName = "etAsset";
                    break;
                default:
                    return "";
            }
            DataTable currTable = RTDS.Tables[tbName];
            DataRow drow = currTable.Rows.Find(username);
            if (drow == null)
            {
                return "";
            }
            else
            {
                string xmlinfo = "";
                foreach (DataColumn column in currTable.Columns)
                {
                    xmlinfo = xmlinfo + column.Caption + ":" + drow[column] + ";";

                }
                return xmlinfo;
            }
        }

        //write a value into the row
        public void WriteRTDataRow(string username, ElemType dtype, string fieldname, string value)
        {
            string tbName = "";
            switch (dtype)
            {
                case ElemType.etZone:
                    tbName = "ZoneInfo";
                    break;
                case ElemType.etPeople:
                    tbName = "PeopleInfo";
                    break;
                case ElemType.etAsset:
                    tbName = "AssetInfo";
                    break;
            }
            DataTable table = RTDS.Tables[tbName];
            DataRow drow = table.Rows.Find(username);
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
        }


        //write the message into the dataset
        public void WriteRTDataRow(string username, ElemType dtype, string messageInfo)
        {
            if (messageInfo.Trim() == "") return;
            string tbName = "";
            switch (dtype)
            {
                case ElemType.etZone:
                    tbName = "ZoneInfo";
                    break;
                case ElemType.etPeople:
                    tbName = "PeopleInfo";
                    break;
                case ElemType.etAsset:
                    tbName = "AssetInfo";
                    break;
            }
            DataTable currTable = RTDS.Tables[tbName];
            DataRow drow = currTable.Rows.Find(username);
            if (drow == null)
            {
                drow = currTable.NewRow();
                drow["UserName"] = username;
                currTable.BeginLoadData();
                currTable.LoadDataRow(drow.ItemArray, true);
                currTable.EndLoadData();
                currTable.AcceptChanges();
            }

            drow = currTable.Rows.Find(username);

            string delimStr = ":;";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = null;

            split = messageInfo.Split(delimiter);
            for (int i = 0; i < split.Length; i = i + 2)
            {
                drow[split[i]] = split[i + 1];
            }
        }
    }
}
