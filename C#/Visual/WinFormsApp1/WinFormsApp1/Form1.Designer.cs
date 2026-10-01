namespace WinFormsApp1
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
            components = new System.ComponentModel.Container();
            lbltest = new Label();
            comboBox1 = new ComboBox();
            button2 = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            copyToolStripMenuItem = new ToolStripMenuItem();
            cutToolStripMenuItem = new ToolStripMenuItem();
            textBox1 = new TextBox();
            contextMenuStrip2 = new ContextMenuStrip(components);
            copyToolStripMenuItem1 = new ToolStripMenuItem();
            cutToolStripMenuItem1 = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            projectSolutionToolStripMenuItem = new ToolStripMenuItem();
            projectFromExistingCOdeToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            cloneRepositoryToolStripMenuItem = new ToolStripMenuItem();
            startWIndowsToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            gitToolStripMenuItem = new ToolStripMenuItem();
            button1 = new Button();
            comboBox2 = new ComboBox();
            txtId = new Label();
            txtName = new Label();
            txtMarks = new Label();
            button3 = new Button();
            button4 = new Button();
            contextMenuStrip1.SuspendLayout();
            contextMenuStrip2.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // lbltest
            // 
            lbltest.AutoSize = true;
            lbltest.BackColor = Color.Transparent;
            lbltest.ForeColor = SystemColors.ControlDarkDark;
            lbltest.Location = new Point(75, 193);
            lbltest.Name = "lbltest";
            lbltest.Size = new Size(59, 25);
            lbltest.TabIndex = 1;
            lbltest.Text = "label1";
            lbltest.Click += label1_Click;
            // 
            // comboBox1
            // 
            comboBox1.AutoCompleteCustomSource.AddRange(new string[] { "Comsats", "Complex" });
            comboBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "1", "2", "3" });
            comboBox1.Location = new Point(36, 100);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(219, 33);
            comboBox1.TabIndex = 2;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // button2
            // 
            button2.AccessibleDescription = "this is button description";
            button2.BackColor = Color.Transparent;
            button2.ContextMenuStrip = contextMenuStrip1;
            button2.Cursor = Cursors.Hand;
            button2.ForeColor = SystemColors.ActiveCaptionText;
            button2.Location = new Point(54, 259);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 3;
            button2.Text = "Sum";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem, cutToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(127, 68);
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(126, 32);
            copyToolStripMenuItem.Text = "Copy";
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.Size = new Size(126, 32);
            cutToolStripMenuItem.Text = "Cut";
            // 
            // textBox1
            // 
            textBox1.ContextMenuStrip = contextMenuStrip2;
            textBox1.Location = new Point(36, 48);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(150, 31);
            textBox1.TabIndex = 5;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // contextMenuStrip2
            // 
            contextMenuStrip2.ImageScalingSize = new Size(24, 24);
            contextMenuStrip2.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem1, cutToolStripMenuItem1, pasteToolStripMenuItem });
            contextMenuStrip2.Name = "contextMenuStrip2";
            contextMenuStrip2.Size = new Size(128, 100);
            contextMenuStrip2.Opening += contextMenuStrip2_Opening;
            // 
            // copyToolStripMenuItem1
            // 
            copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            copyToolStripMenuItem1.Size = new Size(127, 32);
            copyToolStripMenuItem1.Text = "Copy";
            // 
            // cutToolStripMenuItem1
            // 
            cutToolStripMenuItem1.Name = "cutToolStripMenuItem1";
            cutToolStripMenuItem1.Size = new Size(127, 32);
            cutToolStripMenuItem1.Text = "Cut";
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.Size = new Size(127, 32);
            pasteToolStripMenuItem.Text = "paste";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, gitToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(745, 33);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, cloneRepositoryToolStripMenuItem, startWIndowsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { projectSolutionToolStripMenuItem, projectFromExistingCOdeToolStripMenuItem });
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(249, 34);
            newToolStripMenuItem.Text = "New";
            // 
            // projectSolutionToolStripMenuItem
            // 
            projectSolutionToolStripMenuItem.Name = "projectSolutionToolStripMenuItem";
            projectSolutionToolStripMenuItem.Size = new Size(330, 34);
            projectSolutionToolStripMenuItem.Text = "Project Solution";
            // 
            // projectFromExistingCOdeToolStripMenuItem
            // 
            projectFromExistingCOdeToolStripMenuItem.Name = "projectFromExistingCOdeToolStripMenuItem";
            projectFromExistingCOdeToolStripMenuItem.Size = new Size(330, 34);
            projectFromExistingCOdeToolStripMenuItem.Text = "Project From existing COde";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(249, 34);
            openToolStripMenuItem.Text = "Open";
            // 
            // cloneRepositoryToolStripMenuItem
            // 
            cloneRepositoryToolStripMenuItem.Name = "cloneRepositoryToolStripMenuItem";
            cloneRepositoryToolStripMenuItem.Size = new Size(249, 34);
            cloneRepositoryToolStripMenuItem.Text = "Clone Repository";
            // 
            // startWIndowsToolStripMenuItem
            // 
            startWIndowsToolStripMenuItem.Name = "startWIndowsToolStripMenuItem";
            startWIndowsToolStripMenuItem.Size = new Size(249, 34);
            startWIndowsToolStripMenuItem.Text = "Start WIndows";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(58, 29);
            editToolStripMenuItem.Text = "Edit";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(65, 29);
            viewToolStripMenuItem.Text = "View";
            // 
            // gitToolStripMenuItem
            // 
            gitToolStripMenuItem.Name = "gitToolStripMenuItem";
            gitToolStripMenuItem.Size = new Size(50, 29);
            gitToolStripMenuItem.Text = "Git";
            // 
            // button1
            // 
            button1.Location = new Point(167, 210);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 7;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(452, 48);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(182, 33);
            comboBox2.TabIndex = 8;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // txtId
            // 
            txtId.AutoSize = true;
            txtId.Location = new Point(406, 132);
            txtId.Name = "txtId";
            txtId.Size = new Size(59, 25);
            txtId.TabIndex = 9;
            txtId.Text = "label1";
            // 
            // txtName
            // 
            txtName.AutoSize = true;
            txtName.Location = new Point(489, 132);
            txtName.Name = "txtName";
            txtName.Size = new Size(59, 25);
            txtName.TabIndex = 9;
            txtName.Text = "label1";
            // 
            // txtMarks
            // 
            txtMarks.AutoSize = true;
            txtMarks.Location = new Point(554, 132);
            txtMarks.Name = "txtMarks";
            txtMarks.Size = new Size(59, 25);
            txtMarks.TabIndex = 9;
            txtMarks.Text = "label1";
            // 
            // button3
            // 
            button3.Location = new Point(323, 222);
            button3.Name = "button3";
            button3.Size = new Size(112, 34);
            button3.TabIndex = 10;
            button3.Text = "Stopwatch";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(441, 222);
            button4.Name = "button4";
            button4.Size = new Size(171, 34);
            button4.TabIndex = 11;
            button4.Text = "Simple Stopwatch";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            AutoSize = true;
            BackColor = SystemColors.InactiveBorder;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(745, 321);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(txtMarks);
            Controls.Add(txtName);
            Controls.Add(txtId);
            Controls.Add(comboBox2);
            Controls.Add(button1);
            Controls.Add(menuStrip1);
            Controls.Add(textBox1);
            Controls.Add(button2);
            Controls.Add(comboBox1);
            Controls.Add(lbltest);
            DoubleBuffered = true;
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            RightToLeft = RightToLeft.No;
            Text = "Form1";
            TransparencyKey = SystemColors.HotTrack;
            Load += Form1_Load;
            contextMenuStrip1.ResumeLayout(false);
            contextMenuStrip2.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lbltest;
        private ComboBox comboBox1;
        private Button button2;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private TextBox textBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem projectSolutionToolStripMenuItem;
        private ToolStripMenuItem projectFromExistingCOdeToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem cloneRepositoryToolStripMenuItem;
        private ToolStripMenuItem startWIndowsToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem gitToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem copyToolStripMenuItem1;
        private ToolStripMenuItem cutToolStripMenuItem1;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private Button button1;
        private ComboBox comboBox2;
        private Label txtId;
        private Label txtName;
        private Label txtMarks;
        private Button button3;
        private Button button4;
    }
}
