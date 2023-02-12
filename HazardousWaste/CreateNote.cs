using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using PdfSharp.Drawing;
using System.Diagnostics;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace HazardousWaste
{
    public partial class CreateNote : Form
    {
        SqlConnection con = new SqlConnection(@"" + Global.SqlDataSource + "");
        SqlCommand command;
        public CreateNote()
        {
            InitializeComponent();
        }

        string EWC = "";
        string HazCode = "";
        string PhysForm = "";
        string DisposalCode = "";
        string UNnum = "";
        string UNClass = "";
        string ProperShipName = "";
        string Packing = "";
        string Handling = "";
        string Component = "";
        string Concentration = "";
        string overflowstring = "";
        bool canteditconsignmentnoteanymore = false;
        bool OnOneLine = false;
        bool Print;
        string CustomCode = "";

        private void CreateNote_Load(object sender, EventArgs e)
        {
            string query = "SELECT Name FROM Carriers WHERE Deleted = 0 ORDER BY Name DESC";
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "Carriers");
            Carriers.DisplayMember = "Name";
            Carriers.ValueMember = "Name";
            Carriers.DataSource = ds.Tables["Carriers"];
            Carriers.SelectedIndex = -1;

            query = "SELECT Name FROM Customers WHERE Deleted = 0 ORDER BY Name DESC";
            da = new SqlDataAdapter(query, con);
            ds = new DataSet();
            da.Fill(ds, "Producer");
            Producer.DisplayMember = "Name";
            Producer.ValueMember = "Name";
            Producer.DataSource = ds.Tables["Producer"];
            Producer.SelectedIndex = -1;

            query = "SELECT Name FROM Disposal WHERE Deleted = 0 ORDER BY Name DESC";
            da = new SqlDataAdapter(query, con);
            ds = new DataSet();
            da.Fill(ds, "comboBox5");
            comboBox5.DisplayMember = "Name";
            comboBox5.ValueMember = "Name";
            comboBox5.DataSource = ds.Tables["comboBox5"];
            comboBox5.SelectedIndex = -1;

            query = "SELECT Description FROM Item WHERE Deleted = 0 ORDER BY Description DESC";
            da = new SqlDataAdapter(query, con);
            ds = new DataSet();
            da.Fill(ds, "description");
            description.DisplayMember = "Description";
            description.ValueMember = "Description";
            description.DataSource = ds.Tables["description"];
            description.SelectedIndex = -1;
            if (string.Equals(Global.selected_id, "null"))
            {
                // create note

            }
            else
            {
                if (con.State == ConnectionState.Closed) con.Open();
                query = "SELECT TOP 1 * FROM Notes WHERE ConsignmentNote = '" + Global.selected_id + "'";
                command = new SqlCommand(query, con);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ConNumber.Text = reader.GetString(1);
                        PremCode.Text = reader.GetString(2);
                        Production.Text = reader.GetString(3);
                        WasteArise.Text = reader.GetString(4);
                        EstWeight.Value = reader.GetDecimal(5);
                        ConType.Text = reader.GetString(6);
                        Acctweight.Value = reader.GetDecimal(7);
                        if (reader.GetInt32(8) == 1)
                        {
                            Rejected.Checked = true;
                        }
                        else
                        {
                            Rejected.Checked = false;
                        }
                        VRM.Text = reader.GetString(9);
                        Carriers.SelectedIndex = (reader.GetInt32(10) - 1);
                        comboBox5.SelectedIndex = (reader.GetInt32(11) - 1);
                        description.SelectedIndex = (reader.GetInt32(12) - 1);
                        Producer.SelectedIndex = (reader.GetInt32(15) - 1);
                        dateTimePicker1.Value = reader.GetDateTime(16);
                        canteditconsignmentnoteanymore = true;
                        CustomNote.Enabled = false;
                    }
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void CustomNote_CheckedChanged(object sender, EventArgs e)
        {
            if(CustomNote.Checked)
            {
                ConNumber.Enabled = true;
                PremCode.Enabled = true;
            }
            else
            {
                ConNumber.Enabled = false;
                PremCode.Enabled = false;
            }
        }

        private void Producer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetConsNumberAndPrefix();
        }

        private bool HasCustomCode(string value)
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string query = "SELECT TOP 1 PremCode FROM Customers WHERE Name = '" + value + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            { 
                if (reader.Read())
                {
                    CustomCode = reader.GetString(0);
                    return true;
                }
            }
            return false;
        }
        private void SetConsNumberAndPrefix()
        {
            if (!canteditconsignmentnoteanymore && CustomNote.Checked == false)
            {
                if (Production.Text.Length > 3 && (!string.IsNullOrEmpty(Producer.Text)))
                {
                    string prefix = "";
                    string search = "";
                    if (HasCustomCode(Producer.Text))
                    {
                        if (string.IsNullOrEmpty(CustomCode))
                        {
                            prefix = Producer.Text;
                            prefix = prefix.Substring(0, 6);
                            prefix = prefix.Replace(" ", string.Empty);
                            prefix = prefix.ToUpper();
                            PremCode.Text = prefix;
                            search = PremCode.Text + "/" + Production.Text.Substring(0, 3).ToUpper();
                        }
                        else
                        {
                            CustomCode = CustomCode.ToUpper();
                            PremCode.Text = CustomCode;
                            prefix = CustomCode;
                            search = prefix + "/" + Production.Text.Substring(0, 3).ToUpper();
                        }
                    }

                    if (con.State == ConnectionState.Closed) con.Open();
                    string query = "SELECT COUNT(ConsignmentNote) FROM Notes WHERE ConsignmentNote LIKE '" + search + "%' AND Deleted = 0";
                    command = new SqlCommand(query, con);
                    SqlDataReader reader = command.ExecuteReader();
                    string number = "00";

                    int temp;
                    if (reader.Read())
                    {
                        temp = reader.GetInt32(0);
                        temp++;
                        number = temp.ToString("00");

                    }

                    ConNumber.Text = prefix + "/" + Production.Text.Substring(0, 3).ToUpper() + number;
                }
            }
        }

        private void Production_TextChanged(object sender, EventArgs e)
        {
            SetConsNumberAndPrefix();
        }
        private void GenerateNote()
        {
            /* --- data validation --- */
            if (string.IsNullOrEmpty(ConNumber.Text))
            {
                MessageBox.Show("Please enter a consignment note number", "Checks", MessageBoxButtons.OK);
                return;
            }

            if (ConNumber.Text.Length != 12)
            {
                MessageBox.Show("Consignment note number needs to be 12 characters", "Checks", MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(PremCode.Text))
            {
                MessageBox.Show("Please enter a premesis code", "Checks", MessageBoxButtons.OK);
                return;
            }

            if (PremCode.Text.Length != 6)
            {
                MessageBox.Show("Premesis code needs 6 characters", "Checks", MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(WasteArise.Text))
            {
                MessageBox.Show("Please enter the process giving rise to the waste", "Checks", MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(ConType.Text))
            {
                MessageBox.Show("Please enter containment type", "Checks", MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(dateTimePicker1.Text))
            {
                MessageBox.Show("Please enter a date", "Checks", MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(EstWeight.Text))
            {
                MessageBox.Show("Please enter an estimated weight", "Checks", MessageBoxButtons.OK);
                return;

            }

            if (string.IsNullOrEmpty(Production.Text))
            {
                MessageBox.Show("Please enter a removed from address", "Checks", MessageBoxButtons.OK);
                return;

            }

            if (string.IsNullOrEmpty(Producer.Text))
            {
                MessageBox.Show("Please enter a Producer/Consignor address (customer)", "Checks", MessageBoxButtons.OK);
                return;

            }

            if (string.IsNullOrEmpty(Carriers.Text))
            {
                MessageBox.Show("Please enter a carrier", "Checks", MessageBoxButtons.OK);
                return;

            }

            if (string.IsNullOrEmpty(comboBox5.Text))
            {
                MessageBox.Show("Please enter a Consignee address (disposal site)", "Checks", MessageBoxButtons.OK);
                return;

            }

            if (string.IsNullOrEmpty(description.Text))
            {
                MessageBox.Show("Please enter a product", "Checks", MessageBoxButtons.OK);
                return;

            }

            if (string.IsNullOrEmpty(VRM.Text))
            {
                MessageBox.Show("Please enter a vehicle reg", "Checks", MessageBoxButtons.OK);
                return;

            }

            if (ExistingNote(ConNumber.Text) && string.Equals(Global.selected_id, "null"))
            {
                MessageBox.Show("This consignment note number already exists", "Checks", MessageBoxButtons.OK);
                return;
            }
            else
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string query;
                int deletedvalue = 0;
                int aorr;
                if (Rejected.Checked) aorr = 1;
                else aorr = 0;
                if (string.Equals(Global.selected_id, "null")) query = "INSERT INTO Notes (ConsignmentNote, PremisesCode, RemovedAddress, GivingRise, EstQty, Container, ActualQty, AcceptedRejected, VRM, CustomerID, CarrierID, DisposalID, ItemID, Deleted, Date) VALUES('" + ConNumber.Text + "', '" + PremCode.Text + "', '" + Production.Text + "', '" + WasteArise.Text + "', '" + EstWeight.Value + "', '" + ConType.Text + "','" + Acctweight.Value + "', '" + aorr + "', '" + VRM.Text + "', '" + (Producer.SelectedIndex + 1) + "', '" + (Carriers.SelectedIndex + 1) + "', '" + (comboBox5.SelectedIndex + 1) + "', '" + (description.SelectedIndex + 1) + "', '" + deletedvalue + "', '" + dateTimePicker1.Text + "')";
                else query = "UPDATE Notes SET ConsignmentNote = '" + ConNumber.Text + "', PremisesCode = '" + PremCode.Text + "', RemovedAddress = '" + Production.Text + "', GivingRise = '" + WasteArise.Text + "', EstQty = '" + EstWeight.Value + "', Container = '" + ConType.Text + "', ActualQty = '" + Acctweight.Value + "', AcceptedRejected = '" + aorr + "', VRM = '" + VRM.Text + "', CustomerID = '" + (Producer.SelectedIndex + 1) + "', CarrierID = '" + (Carriers.SelectedIndex + 1) + "', DisposalID = '" + (comboBox5.SelectedIndex + 1) + "', ItemID = '" + (description.SelectedIndex + 1) + "', Deleted = '" + deletedvalue + "', Date = '" + dateTimePicker1.Text + "' WHERE ConsignmentNote = '" + Global.selected_id + "'";
                command = new SqlCommand(query, con);
                command.ExecuteNonQuery();
                con.Close();

                SaveNote(ConNumber.Text);
                this.Close();
            }
        }
        private void Generate_Click(object sender, EventArgs e)
        {
            Print = true;
            GenerateNote();
        }

        private bool ExistingNote(string value)
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string query = "SELECT TOP 1 * FROM Notes WHERE ConsignmentNote = '" + value + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows) return true;
                return false;
        }

        private string GetDisposalAddress(string value)
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string rtnstring = "";
            string query = "SELECT TOP 1 * FROM Disposal WHERE Name = '" + value + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            string AddressLine1 = "", AddressLine2 = "", AddressLine3 = "", AddressLine4 = "", Postcode = "";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    AddressLine1 = reader.GetString(2);
                    AddressLine2 = reader.GetString(3);
                    AddressLine3 = reader.GetString(4);
                    AddressLine4 = reader.GetString(5);
                    Postcode = reader.GetString(6);
                }
            }

            if (string.IsNullOrEmpty(AddressLine2) && string.IsNullOrEmpty(AddressLine3) && string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1;
                    overflowstring = Postcode;
                }
                return rtnstring;
            }

            if (string.IsNullOrEmpty(AddressLine3) && string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1;
                    overflowstring = AddressLine2 + ", " + Postcode;
                }
                return rtnstring;
            }

            if (string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + AddressLine3 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2;
                    overflowstring = AddressLine3 + ", " + Postcode;
                }
                return rtnstring;
            }

            rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + AddressLine3 + ", " + AddressLine4 + ", " + Postcode;
            if (rtnstring.Length > 50 && !OnOneLine)
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2;
                overflowstring = AddressLine3 + ", " + AddressLine4 + ", " + Postcode;
            }
            OnOneLine = false;
            return rtnstring;
        }

        private string GetCustomerAddress(string value)
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string rtnstring = "";
            string query = "SELECT TOP 1 * FROM Customers WHERE Name = '" + value + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            string AddressLine1 = "", AddressLine2 = "", AddressLine3 = "", AddressLine4 = "", Postcode = "";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    AddressLine1 = reader.GetString(2);
                    AddressLine2 = reader.GetString(3);
                    AddressLine3 = reader.GetString(4);
                    AddressLine4 = reader.GetString(5);
                    Postcode = reader.GetString(6);
                }
            }

            if (string.IsNullOrEmpty(AddressLine2) && string.IsNullOrEmpty(AddressLine3) && string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1;
                    overflowstring = Postcode;
                }
                return rtnstring;
            }

            if (string.IsNullOrEmpty(AddressLine3) && string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1;
                    overflowstring = AddressLine2 + ", " + Postcode;
                }
                return rtnstring;
            }

            if (string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + AddressLine3 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2;
                    overflowstring = AddressLine3 + ", " + Postcode;
                }
                return rtnstring;
            }

            rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + AddressLine3 + ", " + AddressLine4 + ", " + Postcode;
            if (rtnstring.Length > 50 && !OnOneLine)
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2;
                overflowstring = AddressLine3 + ", " + AddressLine4 + ", " + Postcode;
            }
            OnOneLine = false;
            return rtnstring;
        }

        private string GetCarriersAddress(string value)
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string rtnstring = "";
            string query = "SELECT TOP 1 * FROM Carriers WHERE Name = '" + value + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            string AddressLine1 = "", AddressLine2 = "", AddressLine3 = "", AddressLine4 = "", Postcode = "";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    AddressLine1 = reader.GetString(2);
                    AddressLine2 = reader.GetString(3);
                    AddressLine3 = reader.GetString(4);
                    AddressLine4 = reader.GetString(5);
                    Postcode = reader.GetString(6);
                }
            }

            if(string.IsNullOrEmpty(AddressLine2) && string.IsNullOrEmpty(AddressLine3) && string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1;
                    overflowstring = Postcode;
                }
                return rtnstring;
            }

            if (string.IsNullOrEmpty(AddressLine3) && string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1;
                    overflowstring = AddressLine2 + ", " + Postcode;
                }
                return rtnstring;
            }

            if (string.IsNullOrEmpty(AddressLine4))
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + AddressLine3 + ", " + Postcode;
                if (rtnstring.Length > 50 && !OnOneLine)
                {
                    rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2;
                    overflowstring = AddressLine3 + ", " + Postcode;
                }
                return rtnstring;
            }

            rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2 + ", " + AddressLine3 + ", " + AddressLine4 + ", " + Postcode;
            if (rtnstring.Length > 50 && !OnOneLine)
            {
                rtnstring = value + ", " + AddressLine1 + ", " + AddressLine2;
                overflowstring = AddressLine3 + ", " + AddressLine4 + ", " + Postcode;
            }
            OnOneLine = false;
            return rtnstring;
        }

        private string GetDisposalPermit(string value)
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string rtnstring = "";
            string query = "SELECT TOP 1 Permit FROM Disposal WHERE Name = '" + value + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    rtnstring = reader.GetString(0);
                }
            }
            return rtnstring;
        }

        private string GetCustomerSIC(string value)
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string rtnstring = "";
            string query = "SELECT TOP 1 SIC FROM Customers WHERE Name = '" + value + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    rtnstring = reader.GetString(0);
                }
            }
            return rtnstring;
        }

        private string GetCarrierPermit(string value)
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string rtnstring = "";
            string query = "SELECT TOP 1 Permit FROM Carriers WHERE Name = '" + value + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    rtnstring = reader.GetString(0);
                }
            }
            return rtnstring;
        }



        private void LoadItemStuff()
        {
            if (con.State == ConnectionState.Closed) con.Open();
            string query = "SELECT TOP 1 * FROM Item WHERE Description = '" + description.Text + "' AND Deleted = 0";
            command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    EWC = reader.GetString(2);
                    HazCode = reader.GetString(3);
                    PhysForm = reader.GetString(4);
                    DisposalCode = reader.GetString(5);
                    UNnum = reader.GetString(6);
                    UNClass = reader.GetString(7);
                    ProperShipName = reader.GetString(8);
                    Packing = reader.GetString(9);
                    Handling = reader.GetString(10);
                    Component = reader.GetString(11);
                    Concentration = reader.GetString(12);
                }
            }
        }


        private void SaveNote(string notenumer)
        {
            PdfDocument PDFDoc = PdfReader.Open(Global.CleanPDF, PdfDocumentOpenMode.Import);
            PdfDocument PDFNewDoc = new PdfDocument();
            PdfPage pdfPage = PDFNewDoc.AddPage(PDFDoc.Pages[0]);

            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            XFont font = new XFont("Verdana", 7, XFontStyle.Regular);
            LoadItemStuff(); // load strings


            /* PART A */
            graph.DrawString(notenumer, font, XBrushes.Black, new XRect(145, 70, 0, 0), XStringFormats.Default);
            graph.DrawString(PremCode.Text, font, XBrushes.Black, new XRect(385, 70, 0, 0), XStringFormats.Default);

            if (Production.Text.Length > 75)
            {
                graph.DrawString(Production.Text.Substring(0, 75), font, XBrushes.Black, new XRect(21, 93, 0, 0), XStringFormats.Default);
                graph.DrawString(Production.Text.Substring(76), font, XBrushes.Black, new XRect(21, 93+10, 0, 0), XStringFormats.Default);
            }
            else 
            {
                graph.DrawString(Production.Text, font, XBrushes.Black, new XRect(21, 93, 0, 0), XStringFormats.Default);
            }

            graph.DrawString(GetDisposalAddress(comboBox5.Text), font, XBrushes.Black, new XRect(303, 93, 0, 0), XStringFormats.Default);
            graph.DrawString(overflowstring, font, XBrushes.Black, new XRect(303, 93 + 10, 0, 0), XStringFormats.Default);
            OnOneLine = true;
            graph.DrawString(GetCustomerAddress(Producer.Text), font, XBrushes.Black, new XRect(21, 123, 0, 0), XStringFormats.Default);
           // graph.DrawString(overflowstring, font, XBrushes.Black, new XRect(21, 123 + 10, 0, 0), XStringFormats.Default);

            /* PART B */
            graph.DrawString(WasteArise.Text, font, XBrushes.Black, new XRect(152, 155, 0, 0), XStringFormats.Default);
            graph.DrawString(GetCustomerSIC(Producer.Text), font, XBrushes.Black, new XRect(470, 155, 0, 0), XStringFormats.Default);// carrier
            graph.DrawString(description.Text, font, XBrushes.Black, new XRect(19, 255, 0, 0), XStringFormats.Default);
            graph.DrawString(EWC, font, XBrushes.Black, new XRect(130, 255, 0, 0), XStringFormats.Default);
            graph.DrawString(EstWeight.Value.ToString(), font, XBrushes.Black, new XRect(180, 255, 0, 0), XStringFormats.Default);
            graph.DrawString(Component, font, XBrushes.Black, new XRect(222, 255, 0, 0), XStringFormats.Default);
            graph.DrawString(Concentration, font, XBrushes.Black, new XRect(285, 255, 0, 0), XStringFormats.Default);
            graph.DrawString(PhysForm, font, XBrushes.Black, new XRect(363, 255, 0, 0), XStringFormats.Default);
            graph.DrawString(HazCode, font, XBrushes.Black, new XRect(441, 255, 0, 0), XStringFormats.Default);
            graph.DrawString(ConType.Text, font, XBrushes.Black, new XRect(490, 255, 0, 0), XStringFormats.Default);

            graph.DrawString(EWC, font, XBrushes.Black, new XRect(19, 300, 0, 0), XStringFormats.Default);
            graph.DrawString(UNnum, font, XBrushes.Black, new XRect(73, 300, 0, 0), XStringFormats.Default);
            graph.DrawString(ProperShipName, font, XBrushes.Black, new XRect(172, 300, 0, 0), XStringFormats.Default);
            graph.DrawString(UNClass, font, XBrushes.Black, new XRect(285, 300, 0, 0), XStringFormats.Default);
            graph.DrawString(Packing, font, XBrushes.Black, new XRect(342, 300, 0, 0), XStringFormats.Default);
            graph.DrawString(Handling, font, XBrushes.Black, new XRect(427, 300, 0, 0), XStringFormats.Default);

            /* PART C */
            graph.DrawString(GetCarriersAddress(Carriers.Text), font, XBrushes.Black, new XRect(22, 470, 0, 0), XStringFormats.Default);
            graph.DrawString(overflowstring, font, XBrushes.Black, new XRect(22, 470 + 10, 0, 0), XStringFormats.Default);

            graph.DrawString(GetCarrierPermit(Carriers.Text), font, XBrushes.Black, new XRect(235, 498, 0, 0), XStringFormats.Default);
            graph.DrawString(VRM.Text, font, XBrushes.Black, new XRect(165, 508, 0, 0), XStringFormats.Default);

            /* PART D */
            graph.DrawString(GetCustomerAddress(Producer.Text), font, XBrushes.Black, new XRect(305, 432, 0, 0), XStringFormats.Default);
            graph.DrawString(overflowstring, font, XBrushes.Black, new XRect(305, 432 + 10, 0, 0), XStringFormats.Default);

            if (Production.Text.Length > 75)
            {
                graph.DrawString(Production.Text.Substring(0, 75), font, XBrushes.Black, new XRect(305, 480, 0, 0), XStringFormats.Default);
                graph.DrawString(Production.Text.Substring(76), font, XBrushes.Black, new XRect(305, 490, 0, 0), XStringFormats.Default);

            }
            else
            {
                graph.DrawString(Production.Text, font, XBrushes.Black, new XRect(305, 480, 0, 0), XStringFormats.Default);
            }

            /* PART E */
            if (Acctweight.Value != 0)
            {
                graph.DrawString(EWC, font, XBrushes.Black, new XRect(19, 610, 0, 0), XStringFormats.Default);
                graph.DrawString(Acctweight.Value.ToString(), font, XBrushes.Black, new XRect(144, 610, 0, 0), XStringFormats.Default); ;

                string temp = "";
                if (Rejected.Checked)
                    temp = "Rejected";
                else
                    temp = "Accepted";
                graph.DrawString(temp, font, XBrushes.Black, new XRect(308, 610, 0, 0), XStringFormats.Default);
                graph.DrawString(DisposalCode, font, XBrushes.Black, new XRect(421, 610, 0, 0), XStringFormats.Default);
                graph.DrawString(VRM.Text, font, XBrushes.Black, new XRect(165, 660, 0, 0), XStringFormats.Default);
            }
            graph.DrawString(GetDisposalPermit(comboBox5.Text), font, XBrushes.Black, new XRect(21, 715, 0, 0), XStringFormats.Default);
            graph.DrawString(GetDisposalAddress(comboBox5.Text), font, XBrushes.Black, new XRect(345, 675, 0, 0), XStringFormats.Default);
            graph.DrawString(overflowstring, font, XBrushes.Black, new XRect(345, 675 + 10, 0, 0), XStringFormats.Default);

            string filename = ConNumber.Text.Replace("/", "");

            Directory.CreateDirectory(Global.PDFFolderPathToPrint + Producer.Text + "/");

            PDFNewDoc.Save(Global.PDFFolderPathToPrint + Producer.Text + "/" + filename + ".PDF");
            if(Print) Process.Start(Global.PDFFolderPathToPrint + Producer.Text + "/" + filename + ".PDF");
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Print = false;
            GenerateNote();
        }
    }
}
