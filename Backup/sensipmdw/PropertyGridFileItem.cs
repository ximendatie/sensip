using System;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Collections;
using System.ComponentModel;

namespace Sensip
{
    class PropertyGridFileItem : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, 
            System.IServiceProvider provider, object value)

        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                // open dialog
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "image files(JPeg,Gif,Bmp,etc.)|*.jpg;*.jpeg;*.gif; *.bmp; *.tif; *.tiff; *.png"; 
                dialog.AddExtension = true;
                if (dialog.ShowDialog().Equals(DialogResult.OK))
                    return dialog.FileName;
            }

            return value;

        }
    }


    public abstract class ComboBoxItemTypeConvert : TypeConverter
    {

        public Hashtable _hash = null;
        public ComboBoxItemTypeConvert()
        {
            _hash = new Hashtable();
            GetConvertHash();
        }

        public abstract void GetConvertHash();

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            int[] ids = new int[_hash.Values.Count];
            int i = 0;
            foreach (DictionaryEntry myDE in _hash)
            {
                ids[i++] = (int)(myDE.Key);
            }
            return new StandardValuesCollection(ids);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object v)
        {
            if (v is string)
            {
                foreach (DictionaryEntry myDE in _hash)
                {
                    if (myDE.Value.Equals((v.ToString())))
                       return myDE.Key;
                }

            }
            return base.ConvertFrom(context, culture, v);

        }

        public override object ConvertTo(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture,object v, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                foreach (DictionaryEntry myDE in _hash)
                {
                    if (myDE.Key.Equals(v))
                        return myDE.Value.ToString();
                }
                return "";
            }
            return base.ConvertTo(context, culture, v, destinationType);

        }

        public override bool GetStandardValuesExclusive(
            ITypeDescriptorContext context)
        {
            return false;
        }

    }


    public class PropertyGridZonesItem : ComboBoxItemTypeConvert
    {
        public override void GetConvertHash()
        {
            for (int i = 0; i < Properties.Settings.Default.cfgZones.Count; i++ )
            {
                _hash.Add(i, Properties.Settings.Default.cfgZones[i]);
            }
        }

    }

    public class PropertyGridRFIDReadersItem : ComboBoxItemTypeConvert
    {
        public override void GetConvertHash()
        {
            for (int i = 0; i < Properties.Settings.Default.cfgRFIDReaders.Count; i++ )
            {
                _hash.Add(i, Properties.Settings.Default.cfgRFIDReaders[i]) ;
            }
        }

    }


}
