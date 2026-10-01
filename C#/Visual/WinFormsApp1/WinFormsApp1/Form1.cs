using System.Data;
using System.Xml.Linq;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Marks", typeof(int));

            dt.Rows.Add(101, "st1", 100);
            dt.Rows.Add(102, "st2", 90);
            dt.Rows.Add(103, "st3", 80);
            dt.Rows.Add(104, "st4", 70);


            comboBox2.DataSource = dt;
            comboBox2.DisplayMember = "Name";


            //    List<String> ls = new List<string>
            //    {
            //        "Ali",
            //        "st2",
            //        "st3"
            //    };

            //Dictionary<int, string> d = new Dictionary<int, string>
            //{
            //    {1, "st1" },
            //    {2, "st2" },
            //    {3, "st3" },

            //};

            //comboBox1.DataSource = new BindingSource(d,null);
            //comboBox1.DisplayMember = "Value";
            //comboBox1.ValueMember = "Key";


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                button1.Left = button1.Left + 1;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRowView row = (DataRowView)comboBox2.SelectedItem;

            txtId.Text = row["Id"].ToString();
            txtName.Text = row["Name"].ToString();
            txtMarks.Text = row["Marks"].ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 f  = new Form3();
            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form4 f = new Form4();
            f.Show();
        }
    }
}
