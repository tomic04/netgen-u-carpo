using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.UI.Helpers
{
    internal delegate void InitHandler();
    internal delegate void CompletedHandler();
    internal class SwapAnimator
    {
        Timer timer;
        Point startA, startB;
        Control ctrlA, ctrlB;
        bool swapped = false, started = false;
        Stopwatch sw;
        int speed, distance;
        public event InitHandler Init;
        public event CompletedHandler Completed;
        public SwapAnimator(int speed, int distance)
        {
            timer = new Timer();
            timer.Tick += Timer_Tick; ;
            timer.Interval = 1;
            this.speed = speed;
            this.distance = distance;
        }
        public void Swap(Control ctrlA, Control ctrlB)
        {
            if (started) return;
            Init();

            this.ctrlA = ctrlA;
            this.ctrlB = ctrlB;

            startA = ctrlA.Location;
            startB = ctrlB.Location;

            swapped = false;

            sw = Stopwatch.StartNew();
            timer.Start();
            started = true;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            float t = Math.Min(1f, (float)sw.Elapsed.TotalMilliseconds / speed);

            if (t > 1f) t = 1f;

            double m = 1.0 - Math.Abs(2.0 * t - 1.0);

            if (!swapped && t > 0.5)
            {
                swapped = true;

                int zA = ctrlA.Parent.Controls.GetChildIndex(ctrlA);
                int zB = ctrlB.Parent.Controls.GetChildIndex(ctrlB);

                Control back = (zA > zB) ? ctrlA : ctrlB;
                back.BringToFront();

            }

            double u = m * m * (3 - 2 * m);

            int yOff = (int)Math.Round(distance * u);

            ctrlA.Location = new Point(startA.X, startA.Y - yOff);
            ctrlB.Location = new Point(startB.X, startB.Y + yOff);

            if (t >= 1f) { timer.Stop(); started = false; Completed(); }
        }
    }
}
