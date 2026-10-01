using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                MessageBox.Show((conn.state().toString()));
                string query = "query";
                SqlCommand cmd = new SqlCommand(query, conn);

                int arows = cmd.ExecuteNonQuery();
                if(arows >)
            }
        }
    }
}
