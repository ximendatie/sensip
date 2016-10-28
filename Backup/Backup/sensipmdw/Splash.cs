using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sensip
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        public void SetStatus(string str)
        {
            label1.Text = str;
            label1.Refresh();
        }
    }
}