using System;
using System.ComponentModel;
using System.Text;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Sensip
{
    public class AccountSettings
    {
        public AccountSettings()
        {


        }

        private string _DisplayName;
        private string _UserName;
        private string _Password;
        private string _Description;
        private string _Picture;



        [CategoryAttribute("Account Settings"), DescriptionAttribute("Display Name of the Account"),
        DefaultValueAttribute(" ")]
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
            }
        }

        [CategoryAttribute("Account Settings"), DescriptionAttribute("The SIP User Name of the Account")]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        [CategoryAttribute("Account Settings"), DescriptionAttribute("The SIP Password of the Account")]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        [CategoryAttribute("Account Settings"), DescriptionAttribute("the description of account"),
        DefaultValueAttribute(" ")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }


        [CategoryAttribute("Account Settings"), DescriptionAttribute("the picture of account"),
        EditorAttribute(typeof(PropertyGridFileItem),typeof(System.Drawing.Design.UITypeEditor))]
        public string Picture
        {
            get
            {
                return _Picture;
            }
            set
            {
                _Picture = value;
            }
        }

    }
}
