namespace SQLProject
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtId = new TextBox();
            txtName = new TextBox();
            txtAge = new TextBox();
            button1 = new Button();
            btnDel = new Button();
            dataGridView1 = new DataGridView();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 106);
            label1.Margin = new Padding(8, 0, 8, 0);
            label1.Name = "label1";
            label1.Size = new Size(50, 22);
            label1.TabIndex = 0;
            label1.Text = "Name";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 73);
            label2.Margin = new Padding(8, 0, 8, 0);
            label2.Name = "label2";
            label2.Size = new Size(21, 22);
            label2.TabIndex = 1;
            label2.Text = "Id";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(19, 143);
            label3.Margin = new Padding(16, 0, 16, 0);
            label3.Name = "label3";
            label3.Size = new Size(36, 22);
            label3.TabIndex = 2;
            label3.Text = "Age";
            label3.Click += label3_Click;
            // 
            // txtId
            // 
            txtId.Location = new Point(70, 69);
            txtId.Name = "txtId";
            txtId.Size = new Size(248, 28);
            txtId.TabIndex = 3;
            // 
            // txtName
            // 
            txtName.Location = new Point(70, 103);
            txtName.Name = "txtName";
            txtName.Size = new Size(248, 28);
            txtName.TabIndex = 4;
            txtName.TextChanged += txtId_TextChanged;
            // 
            // txtAge
            // 
            txtAge.Location = new Point(70, 137);
            txtAge.Name = "txtAge";
            txtAge.Size = new Size(248, 28);
            txtAge.TabIndex = 5;
            txtAge.TextChanged += txtAge_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(20, 190);
            button1.Name = "button1";
            button1.Size = new Size(64, 38);
            button1.TabIndex = 6;
            button1.Text = "Add";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // btnDel
            // 
            btnDel.Location = new Point(93, 190);
            btnDel.Name = "btnDel";
            btnDel.Size = new Size(64, 38);
            btnDel.TabIndex = 7;
            btnDel.Text = "Delete";
            btnDel.UseVisualStyleBackColor = true;
            btnDel.Click += btnDel_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(383, 59);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(572, 169);
            dataGridView1.TabIndex = 8;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            // 
            // button2
            // 
            button2.Location = new Point(163, 190);
            button2.Name = "button2";
            button2.Size = new Size(64, 38);
            button2.TabIndex = 9;
            button2.Text = "Update";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(233, 190);
            button3.Name = "button3";
            button3.Size = new Size(64, 38);
            button3.TabIndex = 10;
            button3.Text = "Load";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(413, 439);
            button4.Name = "button4";
            button4.Size = new Size(112, 34);
            button4.TabIndex = 11;
            button4.Text = "Form2";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(145, 429);
            button5.Name = "button5";
            button5.Size = new Size(152, 55);
            button5.TabIndex = 12;
            button5.Text = "Generate Excel Report";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FloralWhite;
            ClientSize = new Size(978, 614);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(dataGridView1);
            Controls.Add(btnDel);
            Controls.Add(button1);
            Controls.Add(txtAge);
            Controls.Add(txtName);
            Controls.Add(txtId);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("Arial Narrow", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(8, 4, 8, 4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtId;
        private TextBox txtName;
        private TextBox txtAge;
        private Button button1;
        private Button btnDel;
        private DataGridView dataGridView1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
    }
}
