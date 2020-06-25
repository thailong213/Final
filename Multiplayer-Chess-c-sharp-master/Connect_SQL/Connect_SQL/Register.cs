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

namespace Connect_SQL
{
    public partial class Register : Form
    {

        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        
        public Register()
        {
            InitializeComponent();
            con.ConnectionString = @"Data Source=ADMIN\SQLEXPRESS;Initial Catalog=User;Integrated Security=True";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "" || txtPassword.Text == "")
                MessageBox.Show("Please fill the information");
            if (txtConfirmPass.Text != txtPassword.Text)
                MessageBox.Show("Password do not match");
            else
            {
                con.Open();
                com.Connection = con;
                {
                    SqlCommand com = new SqlCommand("UserAdd", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Username", txtUsername.Text.TrimEnd());
                    com.Parameters.AddWithValue("@Password", txtPassword.Text.TrimEnd());
                    com.ExecuteNonQuery();
                    MessageBox.Show("Registation is susccessful");
                }
            }
            this.Hide();
            Login a = new Login();
            a.Show();
        }
    }
}
