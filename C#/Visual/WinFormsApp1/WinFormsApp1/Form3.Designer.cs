namespace WinFormsApp1
{
    partial class Form3
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
            timer1 = new System.Windows.Forms.Timer(components);
            txtStopwatch = new Label();
            btnStart = new Button();
            btnStop = new Button();
            btnReset = new Button();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Interval = 1;
            timer1.Tick += timer1_Tick;
            // 
            // txtStopwatch
            // 
            txtStopwatch.AutoSize = true;
            txtStopwatch.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
            txtStopwatch.Location = new Point(200, 80);
            txtStopwatch.Name = "txtStopwatch";
            txtStopwatch.Size = new Size(424, 96);
            txtStopwatch.TabIndex = 0;
            txtStopwatch.Text = "00:00:00.000";
            txtStopwatch.Click += label1_Click;
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Segoe UI", 14F);
            btnStart.Location = new Point(150, 250);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(120, 50);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Segoe UI", 14F);
            btnStop.Location = new Point(340, 250);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(120, 50);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnReset
            // 
            btnReset.Font = new Font("Segoe UI", 14F);
            btnReset.Location = new Point(530, 250);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(120, 50);
            btnReset.TabIndex = 3;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnReset);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(txtStopwatch);
            Name = "Form3";
            Text = "Stopwatch";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Label txtStopwatch;
        private Button btnStart;
        private Button btnStop;
        private Button btnReset;
    }
}