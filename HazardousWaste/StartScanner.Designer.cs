
namespace HazardousWaste
{
    partial class StartScanner
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
            this.button1 = new System.Windows.Forms.Button();
            this.lbDevices = new System.Windows.Forms.ComboBox();
            this.pic_scan = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pic_scan)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(106, 586);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(294, 44);
            this.button1.TabIndex = 4;
            this.button1.Text = "Scan";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbDevices
            // 
            this.lbDevices.FormattingEnabled = true;
            this.lbDevices.Location = new System.Drawing.Point(1015, 468);
            this.lbDevices.Name = "lbDevices";
            this.lbDevices.Size = new System.Drawing.Size(121, 21);
            this.lbDevices.TabIndex = 8;
            this.lbDevices.Visible = false;
            // 
            // pic_scan
            // 
            this.pic_scan.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pic_scan.Location = new System.Drawing.Point(12, 12);
            this.pic_scan.Name = "pic_scan";
            this.pic_scan.Size = new System.Drawing.Size(476, 568);
            this.pic_scan.TabIndex = 7;
            this.pic_scan.TabStop = false;
            this.pic_scan.Click += new System.EventHandler(this.pic_scan_Click);
            this.pic_scan.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_scan_Paint);
            this.pic_scan.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_scan_MouseMove);
            this.pic_scan.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_scan_MouseUp);
            // 
            // StartScanner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 638);
            this.Controls.Add(this.lbDevices);
            this.Controls.Add(this.pic_scan);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StartScanner";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scan to System";
            this.Load += new System.EventHandler(this.StartScanner_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic_scan)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pic_scan;
        private System.Windows.Forms.ComboBox lbDevices;
    }
}