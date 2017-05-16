using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SBUExtractor
{
    public partial class Progressbar : Form
    {
        // if maximum is smaller then Int32.MaxValue then the scalefactor is 1 else it would be smaller
        private double scaleFactor = 1;
        // The Maximum of the Progressbar
        private long MaxValue;
        // the current value of the Progressbar
        private long Value;
        // the unit for the progressed data (shown in label)
        private readonly string Unit;

        // Gets the title of the window and the unit of the progress data (for the label)
        public Progressbar(string label, string unit)
        {
            InitializeComponent();
            this.Text = label;
            Unit = unit;
        }

        private void Progressbar_Load(object sender, EventArgs e)
        {
            this.progressBar1.Minimum = 0;
        }

        // Sets the Maximum of the Progressbar and calculates the scale-factor from long to int
        public void SetMaximum(long max)
        {
            if(max > Int32.MaxValue)
                scaleFactor = (double)Int32.MaxValue/max;

            MaxValue = max;

            this.progressBar1.Maximum = (int) (max * this.scaleFactor);
        }

        // scales the long value to int and set it synchronized to display the Progress
        public void SetValue(long val)
        {
            int intVal = (int) (val * scaleFactor);

            if (intVal < progressBar1.Minimum || intVal > progressBar1.Maximum)
                return;

            Value = val;

            if(this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                                                    {
                                                        this.progressBar1.Value = intVal; 
                                                        RefreshLabel();
                                                    });
                return;
            }

            this.progressBar1.Value = intVal;
            RefreshLabel();
        }

        // writes the label again
        private void RefreshLabel()
        {
            this.label1.Text = string.Format("{0:0,0}", Value) + "/" + string.Format("{0:0,0}", MaxValue) + " " + Unit;
        }
    }
}
