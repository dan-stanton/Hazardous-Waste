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
    public partial class EditCustomer : Form
    {
        SqlConnection con = new SqlConnection(@"" + Global.SqlDataSource + "");
        SqlCommand command;

        public EditCustomer()
        {
            InitializeComponent();
        }

        private void EditCustomer_Load(object sender, EventArgs e)
        {
            if (string.Equals(Global.selected_id, "null"))
            {
                EditCreate.Text = "Create";
            }
            else
            {
                EditCreate.Text = "Adjust";

                if (con.State == ConnectionState.Closed) con.Open();
                string query = "SELECT TOP 1 * FROM Customers WHERE Name = '" + Global.selected_id + "'";
                command = new SqlCommand(query, con);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SiteName.Text = reader.GetString(1);
                        AdressLine1.Text = reader.GetString(2);
                        AdressLine2.Text = reader.GetString(3);
                        AdressLine3.Text = reader.GetString(4);
                        AdressLine4.Text = reader.GetString(5);
                        Postcode.Text = reader.GetString(6);
                        SIC.Text = reader.GetString(7);
                        if (reader.GetInt32(8) == 1) Deleted.Checked = true;
                        else Deleted.Checked = false;
                        PremCode.Text = reader.GetString(9);
                    }
                }
                else
                {
                    this.Close();// error so close
                }
                reader.Close();
                con.Close();
            }
        }

        private void EditCreate_Click(object sender, EventArgs e)
        {
            if (IsFormattingOk())
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string query;
                int temp;
                if (Deleted.Checked) temp = 1;
                else temp = 0;
                if (String.Equals(EditCreate.Text, "Create")) query = "INSERT INTO Customers (Name, Address1, Address2, Address3, Address4, Postcode, Deleted, SIC, PremCode) VALUES('" + SiteName.Text + "', '" + AdressLine1.Text + "', '" + AdressLine2.Text + "', '" + AdressLine3.Text + "', '" + AdressLine4.Text + "', '" + Postcode.Text + "', 0, '" + SIC.Text + "', '"+ PremCode.Text + "')";
                else query = "UPDATE Customers SET Name = '" + SiteName.Text + "', Address1 = '" + AdressLine1.Text + "', Address2 = '" + AdressLine2.Text + "', Address3 = '" + AdressLine3.Text + "', Address4 = '" + AdressLine4.Text + "', Postcode = '" + Postcode.Text + "', Deleted = '" + temp + "', SIC = '" + SIC.Text + "', PremCode = '" + PremCode.Text + "' WHERE Name = '" + Global.selected_id + "'";
                command = new SqlCommand(query, con);
                command.ExecuteNonQuery();
                con.Close();
                this.Close();
            }
        }

        private bool IsFormattingOk()
        {
            if(PremCode.Text.Length > 0 && PremCode.Text.Length != 6)
            {
                MessageBox.Show("Prem code needs to have 6 characters", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(SiteName.Text))
            {
                MessageBox.Show("Enter a Site Name", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(AdressLine1.Text))
            {
                MessageBox.Show("Enter an Address", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(Postcode.Text))
            {
                MessageBox.Show("Enter a Postcode", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(SIC.Text))
            {
                MessageBox.Show("Enter a SIC", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }
    }
}
