namespace WinFormsApp1
{
    partial class Form4
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
            lblStopwatch = new Label();
            btnStart = new Button();
            btnStop = new Button();
            btnReset = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // lblStopwatch
            // 
            lblStopwatch.AutoSize = true;
            lblStopwatch.Font = new Font("Segoe UI", 48F, FontStyle.Bold);
            lblStopwatch.Location = new Point(150, 80);
            lblStopwatch.Name = "lblStopwatch";
            lblStopwatch.Size = new Size(627, 128);
            lblStopwatch.TabIndex = 0;
            lblStopwatch.Text = "00:00:00.000";
            lblStopwatch.Click += lblStopwatch_Click;
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Segoe UI", 16F);
            btnStart.Location = new Point(150, 280);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(140, 60);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Segoe UI", 16F);
            btnStop.Location = new Point(350, 280);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(140, 60);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnReset
            // 
            btnReset.Font = new Font("Segoe UI", 16F);
            btnReset.Location = new Point(550, 280);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(140, 60);
            btnReset.TabIndex = 3;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // timer1
            // 
            timer1.Interval = 10;
            timer1.Tick += timer1_Tick;
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 500);
            Controls.Add(btnReset);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(lblStopwatch);
            Name = "Form4";
            Text = "Simple Stopwatch";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblStopwatch;
        private Button btnStart;
        private Button btnStop;
        private Button btnReset;
        private System.Windows.Forms.Timer timer1;
    }
}
