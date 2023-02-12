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
    public partial class Customers : Form
    {
        SqlConnection con = new SqlConnection(@"" + Global.SqlDataSource + "");
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable table;

        public Customers()
        {
            InitializeComponent();
        }

        public void SearchData(string valueToSearch)
        {
            string query;
            if (!Deleted.Checked) query = "SELECT Name, Address1, Address2, Address3, Address4, Postcode, SIC, PremCode FROM Customers WHERE CONCAT(Name, Address1, Address2, Address3, Address4, Postcode, PremCode) LIKE '%" + valueToSearch + "%' AND Deleted = 0";
            else query = "SELECT Name, Address1, Address2, Address3, Address4, Postcode, SIC, PremCode FROM Customers WHERE CONCAT(Name, Address1, Address2, Address3, Address4, Postcode, PremCode) LIKE '%" + valueToSearch + "%'";
            command = new SqlCommand(query, con);
            adapter = new SqlDataAdapter(command);
            table = new DataTable();
            adapter.Fill(table);
            ItemGrid.DataSource = table;

        }

        private void Customers_Load(object sender, EventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }

        private void Create_Click(object sender, EventArgs e)
        {
            Global.selected_id = "null";
            OpenAdjustItem();
        }

        private void Deleted_CheckedChanged(object sender, EventArgs e)
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

        private void OpenAdjustItem()
        {
            using (EditCustomer form = new EditCustomer())
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

        private void Search_Click(object sender, EventArgs e)
        {
            SearchData(SearchText.Text.ToString());
        }
    }
}
