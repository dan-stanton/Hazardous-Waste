using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HazardousWaste
{
    public partial class Notes : Form
    {
        SqlConnection con = new SqlConnection(@"" + Global.SqlDataSource + "");
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable table;
        public Notes()
        {
            InitializeComponent();
        }

        public void SearchData(string valueToSearch)
        {
            string query;
            if (!Deleted.Checked) query = "SELECT Notes.Date, Notes.ConsignmentNote AS [Note Code], Item.Description, Item.EWC, Notes.ActualQty AS [Actual Quantity], Customers.Name AS [Customer Name], Notes.RemovedAddress AS [Production Address], Disposal.Name AS [Disposal Site], Carriers.Name AS [Carriers Name], Notes.VRM AS [Vehicle Reg] FROM Notes INNER JOIN Carriers ON Notes.CarrierID = Carriers.ID INNER JOIN Disposal ON Notes.DisposalID = Disposal.ID INNER JOIN Item ON Notes.ItemID = Item.ItemID INNER JOIN Customers ON Notes.CustomerID = Customers.ID WHERE CONCAT(Notes.ConsignmentNote, Item.Description, Item.EWC, Notes.ActualQty, Customers.Name, Notes.RemovedAddress, Disposal.Name, Carriers.Name, Notes.VRM, Notes.Date) LIKE '%" + valueToSearch + "%' AND Notes.Deleted = 0 ORDER BY DATE DESC";
            else query = "SELECT Notes.Date, Notes.ConsignmentNote AS [Note Code], Item.Description, Item.EWC, Notes.ActualQty AS [Actual Quantity], Customers.Name AS [Customer Name], Notes.RemovedAddress AS [Production Address], Disposal.Name AS [Disposal Site], Carriers.Name AS [Carriers Name], Notes.VRM AS [Vehicle Reg] FROM Notes INNER JOIN Carriers ON Notes.CarrierID = Carriers.ID INNER JOIN Disposal ON Notes.DisposalID = Disposal.ID INNER JOIN Item ON Notes.ItemID = Item.ItemID INNER JOIN Customers ON Notes.CustomerID = Customers.ID WHERE CONCAT(Notes.ConsignmentNote, Item.Description, Item.EWC, Notes.ActualQty, Customers.Name, Notes.RemovedAddress, Disposal.Name, Carriers.Name, Notes.VRM, Notes.Date) LIKE '%" + valueToSearch + "%' ORDER BY DATE DESC";
            command = new SqlCommand(query, con);
            adapter = new SqlDataAdapter(command);
            table = new DataTable();
            adapter.Fill(table);
            ItemGrid.DataSource = table;
        }


        private void Notes_Load(object sender, EventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void SearchText_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(SearchText.Text)) Search.Enabled = false;
            else Search.Enabled = true;
            SearchData("");
        }

        private void Search_Click(object sender, EventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void Deleted_CheckedChanged(object sender, EventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void ItemGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            ItemGrid.CurrentCell = ItemGrid.Rows[ItemGrid.CurrentCell.RowIndex].Cells[1];
            Global.selected_id = (string)ItemGrid.CurrentCell.Value;
            ItemGrid.CurrentCell = ItemGrid.Rows[ItemGrid.CurrentCell.RowIndex].Cells[5];
            Global.SelectedCustomer = (string)ItemGrid.CurrentCell.Value;
            OpenAdjustItem();
        }

        private void Create_Click(object sender, EventArgs e)
        {
            Global.selected_id = "null";
            using (CreateNote form = new CreateNote())
            {
                form.FormClosing += new FormClosingEventHandler(ChildFormClosing);
                form.ShowDialog();
            }
        }

        private void OpenAdjustItem()
        {
            using (ViewOrEdit form = new ViewOrEdit())
            {
                form.FormClosing += new FormClosingEventHandler(ChildFormClosing);
                form.ShowDialog();
            }
        }

        private void ChildFormClosing(object sender, FormClosingEventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void ItemGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow Myrow in ItemGrid.Rows)
            {
                string customer = Convert.ToString(Myrow.Cells[5].Value);
                string notenumber = Convert.ToString(Myrow.Cells[1].Value);
                notenumber = notenumber.Replace("/", "");
                if (File.Exists(Global.CompletedPDFPath + customer + "\\" + notenumber + ".pdf"))
                {
                    ItemGrid.Rows[Myrow.Index].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(143, 255, 102);
                }
            }
        }
    }
}
