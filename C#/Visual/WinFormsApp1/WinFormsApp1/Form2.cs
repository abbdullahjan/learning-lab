using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        private List<Point> trail = new List<Point>();

        public Form2()
        {
            InitializeComponent();
            this.DoubleBuffered = true;  // Prevent flickering
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num1, num2, result;
            if (int.TryParse(textBox1.Text, out num1) && int.TryParse(textBox2.Text, out num2))
            {
                result = num1 + num2;
                label4.Text = result.ToString();
            }
        }

        double angle = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            double radius = 150;                        // circle size
            int centerX = button1.Left;                          // center point X
            int centerY = button1.Top;                          // center point Y
           
            
            double r = 100 * Math.Cos(6 * angle);
            int x = (int)(centerX + r * Math.Cos(angle));
            int y = (int)(centerY + r * Math.Sin(angle));


            //5 flower
            //double r = 100 * Math.Cos(5 * angle);
            //int x = (int)(centerX + r * Math.Cos(angle));
            //int y = (int)(centerY + r * Math.Sin(angle));

            //circle
            //int x = (int)(centerX + radius * Math.Cos(angle));
            //int y = (int)(centerY + radius * Math.Sin(angle));

            Point trailPoint = new Point(x + movable.Width / 2, y + movable.Height / 2);
            trail.Add(trailPoint);

            if (trail.Count > 5000)
            {
                trail.RemoveAt(0);
            }

            movable.Location = new Point(x, y);

            angle += 0.05;
            
            // Trigger repaint to draw the trail
            this.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (trail.Count > 1)
            {
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    // Draw lines connecting all trail points
                    for (int i = 0; i < trail.Count - 1; i++)
                    {
                        e.Graphics.DrawLine(pen, trail[i], trail[i + 1]);
                    }
                }
            }
        }
    }
}
