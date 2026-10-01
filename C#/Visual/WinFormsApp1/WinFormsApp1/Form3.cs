
using System.Diagnostics;  


namespace WinFormsApp1
{
    public partial class Form3 : Form
    {
        private Stopwatch stopwatch = new Stopwatch();

        public Form3()
        {
            InitializeComponent();
            txtStopwatch.Text = "00:00:00.000";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            stopwatch.Start();  
            timer1.Start();    
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopwatch.Stop();    
            timer1.Stop();       
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            stopwatch.Reset();              
            txtStopwatch.Text = "00:00:00.000";  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = stopwatch.Elapsed;

            txtStopwatch.Text = string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                elapsed.Hours,
                elapsed.Minutes,
                elapsed.Seconds,
                elapsed.Milliseconds);
        }
    }
}
