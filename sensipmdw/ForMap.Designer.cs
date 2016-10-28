namespace Sensip
{
    partial class ForMap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ForMap));
            this.mySVG = new AxSVGACTIVEXLib.AxSVGCtl();
            this.zomOut = new System.Windows.Forms.Button();
            this.zoomIn = new System.Windows.Forms.Button();
            this.openFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.g040 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.mySVG)).BeginInit();
            this.SuspendLayout();
            // 
            // mySVG
            // 
            this.mySVG.Enabled = true;
            this.mySVG.Location = new System.Drawing.Point(12, 12);
            this.mySVG.Name = "mySVG";
            this.mySVG.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("mySVG.OcxState")));
            this.mySVG.Size = new System.Drawing.Size(462, 271);
            this.mySVG.TabIndex = 1;
            // 
            // zomOut
            // 
            this.zomOut.Location = new System.Drawing.Point(117, 347);
            this.zomOut.Name = "zomOut";
            this.zomOut.Size = new System.Drawing.Size(88, 24);
            this.zomOut.TabIndex = 7;
            this.zomOut.Text = "Zoom Out";
            // 
            // zoomIn
            // 
            this.zoomIn.Location = new System.Drawing.Point(23, 347);
            this.zoomIn.Name = "zoomIn";
            this.zoomIn.Size = new System.Drawing.Size(88, 24);
            this.zoomIn.TabIndex = 6;
            this.zoomIn.Text = "Zoom In";
            // 
            // openFile
            // 
            this.openFile.Location = new System.Drawing.Point(23, 317);
            this.openFile.Name = "openFile";
            this.openFile.Size = new System.Drawing.Size(96, 24);
            this.openFile.TabIndex = 8;
            this.openFile.Text = "Open Map";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "SVG Maps (*.svg)|*.svg";
            this.openFileDialog.InitialDirectory = "C:\\My Code\\Map Display";
            // 
            // g040
            // 
            this.g040.Location = new System.Drawing.Point(329, 320);
            this.g040.Name = "g040";
            this.g040.Size = new System.Drawing.Size(87, 25);
            this.g040.TabIndex = 9;
            this.g040.Text = "SelectRoom";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(431, 323);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(132, 20);
            this.textBox1.TabIndex = 10;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(480, 11);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(120, 264);
            this.listBox1.TabIndex = 11;
            // 
            // ForMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 397);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.g040);
            this.Controls.Add(this.openFile);
            this.Controls.Add(this.zomOut);
            this.Controls.Add(this.zoomIn);
            this.Controls.Add(this.mySVG);
            this.Name = "ForMap";
            this.Text = "ForMap";
            ((System.ComponentModel.ISupportInitialize)(this.mySVG)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxSVGACTIVEXLib.AxSVGCtl mySVG;
        private System.Windows.Forms.Button zomOut;
        private System.Windows.Forms.Button zoomIn;
        private System.Windows.Forms.Button openFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button g040;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox listBox1;
    }
}