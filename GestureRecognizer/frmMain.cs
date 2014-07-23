using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class frmMain : Form
    {
        private int      count;
        private DateTime startTime;
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
        }

        private void tmrRecognize_Tick(object sender, EventArgs e)
        {
            if (Form.MouseButtons != MouseButtons.None)
            {
                if (GestureRecognizer.Recognize(Form.MousePosition) == true)
                {
                    count++;
                }
            }
            else
            {
                count = 0;
                GestureRecognizer.Reset();
            }
            
            if (count == 50)
            {
                tmrRecognize.Enabled = false;
                
                MessageBox.Show("Your record : " + (DateTime.Now - startTime).TotalSeconds.ToString());
                Application.Exit();
            }
        }

        private void tmrTime_Tick(object sender, EventArgs e)
        {
            Text = "Count : " + count + ", Time Elapsed : " + (DateTime.Now - startTime).TotalSeconds.ToString();
        }
    }
}
