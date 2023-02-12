using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HazardousWaste
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private Form activeForm = null;
        private void OpenChildForm(Form childForm)
        {
            if (activeForm != null) activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            MainPanel.Controls.Add(childForm);
            MainPanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

        }

        private void ItemBtn_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Items());
        }

        private void DisposalBtn_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Disposal());
        }

        private void CarriersBtn_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Carriers());
        }

        private void CusBtn_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Customers());
        }

        private void NotesBtn_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Notes());
        }

        private void Main_Load(object sender, EventArgs e)
        {
            bool BetaMode = true;
            if(BetaMode == true)
            {
                Global.SqlDataSource = "Data Source=.;Initial Catalog=HazWaste;Persist Security Info=True;User ID=StoresUser;Password=redacted;MultipleActiveResultSets=true";
                Global.CleanPDF = "C:\\Users\\relle\\Desktop\\Projects\\HazardousWaste\\CleanCopy.pdf";
                Global.PDFFolderPathToPrint = "C:\\Users\\relle\\Desktop\\pdf\\";
                Global.CompletedPDFPath = "C:\\Users\\relle\\Desktop\\scanned\\";
            }
            else
            {
                Global.SqlDataSource = "Data Source=redacted;Initial Catalog=HazWaste;Persist Security Info=True;User ID=StoresUser;Password=redacted;MultipleActiveResultSets=true";
                Global.CleanPDF = "W:\\SOFTWARE\\Source Code\\HazardousWaste\\CleanCopy.pdf";
                Global.PDFFolderPathToPrint = "W:\\SOFTWARE\\Source Code\\HazardousWaste\\FreshNotes\\";
                Global.CompletedPDFPath = "W:\\SOFTWARE\\Source Code\\HazardousWaste\\Completed Notes\\";
            }
        }

        private void ReportsBtn_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Reports());
        }
    }
}
