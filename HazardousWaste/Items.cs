using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HazardousWaste
{
    public partial class Items : Form
    {
        SqlConnection con = new SqlConnection(@"" + Global.SqlDataSource + "");
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable table;

        public Items()
        {
            InitializeComponent();
        }

        public void SearchData(string valueToSearch)
        {
            string query;
            if (!Deleted.Checked) query = "SELECT Description, EWC, HazCode AS [Hazardous Code], PhysForm AS [Physical Form], DisposalCode AS [Disposal Code], UNnum AS [UN Number], UNClass AS [UN Class],  ProperShipName AS [Propper Shipping Name], PackingGroup AS [Packing Group], SpecialHandlingReq AS [Special Handling], Component, Concentration FROM Item WHERE Description LIKE '%" + valueToSearch + "%' AND Deleted = 0";
            else query = "SELECT Description, EWC, HazCode AS [Hazardous Code], PhysForm AS [Physical Form], DisposalCode AS [Disposal Code], UNnum AS [UN Number], UNClass AS [UN Class],  ProperShipName AS [Propper Shipping Name], PackingGroup AS [Packing Group], SpecialHandlingReq AS [Special Handling], Component, Concentration FROM Item WHERE Description LIKE '%" + valueToSearch + "%'";
            command = new SqlCommand(query, con);
            adapter = new SqlDataAdapter(command);
            table = new DataTable();
            adapter.Fill(table);
            ItemGrid.DataSource = table;

        }

        private void Items_Load(object sender, EventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void Deleted_CheckedChanged(object sender, EventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void Search_Click(object sender, EventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void ItemGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            ItemGrid.CurrentCell = ItemGrid.Rows[ItemGrid.CurrentCell.RowIndex].Cells[0];
            Global.selected_id = (string)ItemGrid.CurrentCell.Value;
            OpenAdjustItem();
        }

        private void Create_Click(object sender, EventArgs e)
        {
            Global.selected_id = "null";
            OpenAdjustItem();
        }

        private void OpenAdjustItem()
        {
            using (EditItem form = new EditItem())
            {
                form.FormClosing += new FormClosingEventHandler(ChildFormClosing);
                form.ShowDialog();
            }
        }

        private void ChildFormClosing(object sender, FormClosingEventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void SearchText_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(SearchText.Text)) Search.Enabled = false;
            else Search.Enabled = true;
            SearchData("");
        }
    }
}
