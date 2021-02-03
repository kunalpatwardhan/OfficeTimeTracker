using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfficeTimeTracker
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        long activeCountThreshold = 60 * 5;
        long idleTimethreshold = 2 * 60;
        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();
        internal struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }
        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }
        /// <summary>
        /// Get the Last input time in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }
            return lastInPut.dwTime;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        string prevTitle = "";
        long activeCount = 0;
        long idleTime = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            string currTitle = "";
            
            currTitle = GetActiveWindowTitle();
            if (currTitle == null)
                currTitle = "";
            if(currTitle==prevTitle)
            {
                activeCount++;
                idleTime++;
                Text = prevTitle + " - " + (activeCount / 1).ToString() + " s";
            }
            else
            {
                if(activeCount > activeCountThreshold)
                {
                    listBox1.Items.Insert (0,prevTitle + " - " + (activeCount / 60).ToString() + " m");
                    listBox1.SelectedIndex = 0;
                }
                    
                activeCount = 0;
                prevTitle = currTitle;
            }
           // idleTime = GetLastInputTime();
          //  TimeSpan timespent = TimeSpan.FromMilliseconds(idleTime);

            if (idleTime > idleTimethreshold)
            {
                try
                {
                    SendKeys.Send("{NUMLOCK}");
                    SendKeys.Send("{NUMLOCK}");
                }
                catch(Exception ex)
                {

                }
                if (listBox1.Items.Count > 0)
                {
                    if (listBox1.Items[0].ToString().Contains(currTitle))
                    {
                        listBox1.Items[0] = prevTitle + " - " + (idleTime / 60).ToString() + " m";
                    }
                    else
                    {
                        listBox1.Items.Insert(0, prevTitle + " - " + (idleTime / 60).ToString() + " m");

                    }
                }
                else
                {
                    listBox1.Items.Insert(0, prevTitle + " - " + (idleTime / 60).ToString() + " m");

                }
                listBox1.SelectedIndex = 0;
                idleTime = 0;
            }
        }
    }
}
