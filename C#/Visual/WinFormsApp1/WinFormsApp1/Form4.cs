using System;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form4 : Form
    {
        private int milliseconds = 0;

        public Form4()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            timer1.Start();   
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();  
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            timer1.Stop();              
            milliseconds = 0;           
            lblStopwatch.Text = "00:00:00.000";  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            milliseconds++;

            int hours = milliseconds / 3600000;             
            int minutes = (milliseconds % 3600000) / 60000;  
            int seconds = (milliseconds % 60000) / 1000;    
            int ms = milliseconds % 1000;                    

            lblStopwatch.Text = string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                hours, minutes, seconds, ms);
        }

        private void lblStopwatch_Click(object sender, EventArgs e)
        {

        }
    }
}
