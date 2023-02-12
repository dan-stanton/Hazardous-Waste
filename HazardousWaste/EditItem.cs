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
    public partial class EditItem : Form
    {
        SqlConnection con = new SqlConnection(@"" + Global.SqlDataSource + "");
        SqlCommand command;

        public EditItem()
        {
            InitializeComponent();
        }

        private void EditItem_Load(object sender, EventArgs e)
        {
            if (string.Equals(Global.selected_id, "null"))
            {
                EditCreate.Text = "Create";
            }
            else
            {
                EditCreate.Text = "Adjust";

                if (con.State == ConnectionState.Closed) con.Open();
                string query = "SELECT TOP 1 * FROM Item WHERE Description = '" + Global.selected_id + "'";
                command = new SqlCommand(query, con);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Description.Text = reader.GetString(1);
                        EWC.Text = reader.GetString(2);
                        HazCode.Text = reader.GetString(3);
                        PhysForm.Text = reader.GetString(4);
                        RDCode.Text = reader.GetString(5);
                        UNNum.Text = reader.GetString(6);
                        UNClass.Text = reader.GetString(7);
                        ShipName.Text = reader.GetString(8);
                        Packing.Text = reader.GetString(9);
                        Handling.Text = reader.GetString(10);
                        Component.Text = reader.GetString(11);
                        Concentration.Text = reader.GetString(12);
                        if (reader.GetInt32(13) == 1) Deleted.Checked = true;
                        else Deleted.Checked = false;
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
                if (String.Equals(EditCreate.Text, "Create")) query = "INSERT INTO Item (Description, EWC, HazCode, PhysForm, DisposalCode, UNnum, UNClass, ProperShipName, PackingGroup, SpecialHandlingReq, Component, Concentration, Deleted) VALUES('" + Description.Text + "', '" + EWC.Text + "', '" + HazCode.Text + "', '" + PhysForm.Text + "', '" + RDCode.Text + "', '" + UNNum.Text + "', '" + UNClass.Text + "', '" + ShipName.Text + "', '" + Packing.Text + "', '" + Handling.Text + "', '" + Component.Text + "', '" + Concentration.Text + "', 0)";
                else query = "UPDATE Item SET Description = '" + Description.Text + "', EWC = '" + EWC.Text + "', HazCode = '" + HazCode.Text + "', PhysForm = '" + PhysForm.Text + "', DisposalCode = '" + RDCode.Text + "', UNnum = '" + UNNum.Text + "', UNClass = '" + UNClass.Text + "', ProperShipName = '" + ShipName.Text + "', PackingGroup = '" + Packing.Text + "', SpecialHandlingReq = '" + Handling.Text + "', Component = '" + Component.Text + "', Concentration = '" + Concentration.Text + "', Deleted = '"+ temp +"' WHERE Description = '"+Global.selected_id+"'";
                command = new SqlCommand(query, con);
                command.ExecuteNonQuery();
                con.Close();
                this.Close();
            }
        }

        private bool IsFormattingOk()
        {
            if (string.IsNullOrEmpty(Description.Text))
            {
                MessageBox.Show("Enter a description", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(EWC.Text))
            {
                MessageBox.Show("Enter a EWC Code", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(HazCode.Text))
            {
                MessageBox.Show("Enter a Hazardous Code", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(PhysForm.Text))
            {
                MessageBox.Show("Enter a Physical Form", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(RDCode.Text))
            {
                MessageBox.Show("Enter a Recovery / Disposal Code", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(UNNum.Text))
            {
                MessageBox.Show("Enter a UN Number", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(UNClass.Text))
            {
                MessageBox.Show("Enter a UN Class", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(ShipName.Text))
            {
                MessageBox.Show("Enter a Shipping Name", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(Packing.Text))
            {
                MessageBox.Show("Enter a Packing Group", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(Handling.Text))
            {
                MessageBox.Show("Enter any Special Handling Requirements", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(Component.Text))
            {
                MessageBox.Show("Enter the Hazardous Component", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(Concentration.Text))
            {
                MessageBox.Show("Enter the Concentration", "Formatting Error", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }
    }
}
