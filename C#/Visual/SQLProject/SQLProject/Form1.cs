using Microsoft.Data.SqlClient;
using System.Data;
using System.Web;
using System.Windows.Forms.VisualStyles;
using ClosedXML.Excel;

namespace SQLProject
{
    public partial class Form1 : Form
    {
        string conStr = "Server=WIN-V8QTVHJSO2F;Database=StudentDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
        //string conStr = "Data Source=WIN-V8QTVHJSO2F;Initial Catalog=StudentDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";
        SqlDataAdapter adapter;
        DataTable table;
        SqlCommandBuilder builder;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            showTable();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            insertValues();
            showTable();
        }

        public void insertValues()
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();

                //MessageBox.Show(conn.State.ToString());

                string query = $"INSERT INTO student VALUES ({txtId.Text}, '{txtName.Text}', '{txtAge.Text}');";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    int arows = cmd.ExecuteNonQuery();

                    if (arows > 0)
                    {
                        MessageBox.Show("Query executed successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No rows were affected.");
                    }
                }
            }

        }

        public void deleteValues(int id)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = $"Delete from student where id = {id}";
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                int arows = cmd.ExecuteNonQuery();
                if (arows > 0)
                {
                    MessageBox.Show($"Query Executed: Query={query}");
                }
                else
                {
                    MessageBox.Show("Query Execution Error");
                }


            }
        }

        public void showTable()
        {
            string query = $"Select * from student";
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand(query, con);
            adapter = new SqlDataAdapter(cmd);
            builder = new SqlCommandBuilder(adapter);

            table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            deleteValues(Convert.ToInt32(txtId.Text));
            showTable();
        }

        private void txtAge_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();

                //MessageBox.Show(conn.State.ToString());

                string query = $"UPDATE student SET Name='{txtName.Text}', Age={txtAge.Text} WHERE Id={Convert.ToInt32(txtId.Text)};";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    int arows = cmd.ExecuteNonQuery();

                    if (arows > 0)
                    {
                        MessageBox.Show("Query executed successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No rows were affected.");
                    }
                }
            }
            showTable();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            showTable();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataRow row = table.Rows[e.RowIndex];
            row.EndEdit();
            adapter.Update(table);
            table.AcceptChanges();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files|*.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Students");

                // Headers
                for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    ws.Cell(1, c + 1).Value = dataGridView1.Columns[c].HeaderText;

                // Data
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                        ws.Cell(r + 2, c + 1).Value = dataGridView1.Rows[r].Cells[c].Value?.ToString();

                wb.SaveAs(sfd.FileName);

                MessageBox.Show("Excel file saved successfully!");
            }
        }  
    }
}
