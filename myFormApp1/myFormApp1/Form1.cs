using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace myFormApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Display List

        private void display()
        {

            SqlConnection connect = new SqlConnection();
            connect.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=FormAPPDB1;Integrated Security=True";

            String Query = "SELECT * FROM Customers";
            SqlCommand cmd = new SqlCommand(Query, connect);
            connect.Open();
            SqlDataReader read = cmd.ExecuteReader();

            if (read.HasRows == true)
            {
                this.dataGridView1.Rows.Clear();
                while (read.Read())
                {
                    this.dataGridView1.Rows.Add(read[0], read[1], read[2], read[3]);
                }
                connect.Close();
            } else
            {
                MessageBox.Show("There is no DATA!!");
            }
            this.firstNameInput.Focus();
        }

        // New

        private void clearBtn_Click(object sender, EventArgs e)
        {
            check = false;
            this.idInput.Text = string.Empty;
            this.firstNameInput.Text = string.Empty;
            this.lastNameInput.Text = string.Empty;
            this.cityInput.Text = string.Empty;

            this.idInput.Enabled = false;

            int idSelector = this.dataGridView1.Rows.Count - 1;
            this.idInput.Text = (Convert.ToInt32(this.dataGridView1.Rows[idSelector].Cells[0].Value.ToString()) + 1).ToString();
            this.firstNameInput.Focus();
        }

        // Create

        private void addBtn_Click(object sender, EventArgs e)
        {

            if(check == true)
                return;

            if (!CheckInfo())
            {
                MessageBox.Show("Fill all the fields before Adding a new customer! ");
                return;
            }

            using (SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=FormAPPDB1;Integrated Security=True"))
            {
                String Query = "INSERT INTO Customers VALUES (" + this.idInput.Text.Trim()+",'"+this.firstNameInput.Text.Trim().ToUpper()+"'," +
                    "'" + this.lastNameInput.Text.Trim().ToUpper() + "','" + this.cityInput.Text.Trim().ToUpper() + "')";
                SqlCommand cmd = new SqlCommand(Query, connect);

                if (connect.State == ConnectionState.Open)
                    connect.Close();
                connect.Open();
                int a = cmd.ExecuteNonQuery();
                display();
                MessageBox.Show("the customer has been added to the list!");
                connect.Close();
                clearBtn_Click(sender, e);
                this.firstNameInput.Focus();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            display();
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;

            int idSelector = this.dataGridView1.Rows.Count - 1;
            this.idInput.Text = (Convert.ToInt32(this.dataGridView1.Rows[idSelector].Cells[0].Value.ToString()) + 1).ToString();
            this.firstNameInput.Focus();
            this.idInput.Enabled = false;
        }

        private bool CheckInfo()
        {
            if (this.idInput.Text.Trim().Equals(string.Empty) ||
                this.firstNameInput.Text.Trim().Equals(string.Empty) ||
                this.lastNameInput.Text.Trim().Equals(string.Empty) ||
                this.cityInput.Text.Trim().Equals(string.Empty))
                return false;
            return true;
        }

        int selectedRow = -1, CustomersId;
        bool check = false;

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = this.dataGridView1.CurrentRow.Index;
            CustomersId = int.Parse(this.dataGridView1.Rows[selectedRow].Cells[0].Value.ToString());

            this.idInput.Text = this.dataGridView1.Rows[selectedRow].Cells[0].Value.ToString();
            this.firstNameInput.Text = this.dataGridView1.Rows[selectedRow].Cells[1].Value.ToString();
            this.lastNameInput.Text = this.dataGridView1.Rows[selectedRow].Cells[2].Value.ToString();
            this.cityInput.Text = this.dataGridView1.Rows[selectedRow].Cells[3].Value.ToString();

            this.idInput.Enabled = false;
            check = true;
        }

        // Modify 

        private void editBtn_Click(object sender, EventArgs e)
        {
            String Query = "UPDATE Customers SET firstName = @p1 , lastName = @p2 , city = @p4 WHERE ID = @p3";

            using (SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=FormAPPDB1;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(Query, connect);
                cmd.Parameters.AddWithValue("@p1", this.firstNameInput.Text.Trim());
                cmd.Parameters.AddWithValue("@p2", this.lastNameInput.Text.Trim());
                cmd.Parameters.AddWithValue("@p3", this.idInput.Text.Trim());
                cmd.Parameters.AddWithValue("@p4", this.cityInput.Text.Trim());
                connect.Open();
                cmd.ExecuteNonQuery();
                connect.Close();
                display();
                MessageBox.Show("the customer has been modified!!");
                clearBtn_Click(sender, e);
            }
            }

        // delete 

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            if(selectedRow == -1)
            {
                MessageBox.Show("Select a line please!");
                return;
            }

            using (SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=FormAPPDB1;Integrated Security=True"))
            {
                DialogResult dialog = MessageBox.Show("Are you sure would you like to delete this customer from the list?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.No)
                    return;

                String Query = "DELETE FROM Customers WHERE ID = " + CustomersId;
                SqlCommand cmd = new SqlCommand(Query, connect);
                connect.Open();
                cmd.ExecuteNonQuery();
                this.dataGridView1.Rows.RemoveAt(selectedRow);
                MessageBox.Show("The selected customer has been deleted!!");
                connect.Close();
                clearBtn_Click(sender, e);
            }

        }
    }
}
