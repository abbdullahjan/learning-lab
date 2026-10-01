using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace SQLProject
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        SqlCommandBuilder builder;
        SqlDataAdapter adapter;
        DataTable dt;
        string conString = "Server=WIN-V8QTVHJSO2F;Database=StudentDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
        //string conString = "Server=WIN-V8QTVHJSO2F database=StudentDB integrated security=true encrypt=true trustservercertificate=true";
        private void Form2_Load(object sender, EventArgs e)
        {

        }

        bool test = false;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (test)
            {
                SqlConnection con = new SqlConnection(conString);
                string query = $"Select * from student where name='{comboBox1.Text}' ";
                con.Open();
                adapter = new SqlDataAdapter(query, con);
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                builder = new SqlCommandBuilder(adapter);
                dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;

            }
            test = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "select distinct name from student";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataAdapter adapterr = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapterr.Fill(dt);
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "name";
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            adapter.Update(dt);
        }
    }
}
