using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace HazardousWaste
{
    public partial class ViewOrEdit : Form
    {
        public ViewOrEdit()
        {
            InitializeComponent();
        }

        private void ViewOrEdit_Load(object sender, EventArgs e)
        {
            notetext.Text = Global.selected_id;

            string customer = Global.SelectedCustomer;
            string notenumber = notetext.Text;
            notenumber = notenumber.Replace("/", "");
            if (File.Exists(Global.CompletedPDFPath + customer + "\\" + notenumber + ".pdf"))
            {
                button1.Text = "View Note";
            }
            else
            {
                button1.Text = "Scan Note";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (CreateNote form = new CreateNote())
            {
                form.ShowDialog();
            }
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string temp = Global.selected_id.Replace("/", "");
            if (File.Exists(Global.CompletedPDFPath + Global.SelectedCustomer + "\\"+ temp +".pdf"))
            {
                Process.Start(Global.CompletedPDFPath + Global.SelectedCustomer + "\\" + temp + ".pdf");
            }
            else
            {
                using (StartScanner form = new StartScanner())
                {
                    form.ShowDialog();
                }
                this.Close();
            }
        }
    }
}
