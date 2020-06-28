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
    public partial class Login : Form
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();

        public Login()
        {
            InitializeComponent();
            con.ConnectionString = @"Data Source=DESKTOP-R3TAL84\MSSQLSERVER01;
                                     Initial Catalog=User;
                                     Integrated Security=True";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from AUTH " +
                              "where Username = '" + txtUsername.Text + 
                              "' AND Password = '" + txtPassword.Text + "'";



            SqlDataReader dr = com.ExecuteReader();
            while (dr.Read())
            {
                for(int i=0 ; i<=dr.FieldCount ; i++)
                {
                    if (txtUsername.Text.Equals(dr["Username"].ToString().TrimEnd()) &&
                        txtPassword.Text.Equals(dr["Password"].ToString().TrimEnd()))
                    {
                        MessageBox.Show("Congrates", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Fail", "fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
            con.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Register a = new Register();
            a.Show();
            this.Hide();
        }
    }
}
