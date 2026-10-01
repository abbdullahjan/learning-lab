namespace WinFormsApp1
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            button1 = new Button();
            label1 = new Label();
            label4 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            movable = new Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(120, 99);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(150, 31);
            textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(120, 169);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(150, 31);
            textBox2.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 175);
            label2.Name = "label2";
            label2.Size = new Size(92, 25);
            label2.TabIndex = 3;
            label2.Text = "Number 2";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(33, 102);
            label3.Name = "label3";
            label3.Size = new Size(92, 25);
            label3.TabIndex = 4;
            label3.Text = "Number 1";
            label3.Click += label3_Click;
            // 
            // button1
            // 
            button1.Location = new Point(375, 131);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 5;
            button1.Text = "Add";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(265, 296);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 6;
            label1.Text = "Result";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(357, 296);
            label4.Name = "label4";
            label4.Size = new Size(0, 25);
            label4.TabIndex = 7;
            label4.Click += label4_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 20;
            timer1.Tick += timer1_Tick;
            // 
            // movable
            // 
            movable.AccessibleName = "";
            movable.Location = new Point(375, 29);
            movable.Name = "movable";
            movable.Size = new Size(112, 34);
            movable.TabIndex = 8;
            movable.Text = "button";
            movable.UseVisualStyleBackColor = true;
            movable.Click += button2_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(movable);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label2;
        private Label label3;
        private Button button1;
        private Label label1;
        private Label label4;
        private System.Windows.Forms.Timer timer1;
        private Button movable;
    }
}