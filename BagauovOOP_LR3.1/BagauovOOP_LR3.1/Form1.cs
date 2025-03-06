using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BagauovOOP_LR3._1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = numericUpDown1.Value.ToString();
            trackBar1.Value = (int)numericUpDown1.Value;
            
            textBox2.Text = numericUpDown2.Value.ToString();
            trackBar2.Value = (int)numericUpDown2.Value;
            
            textBox3.Text = numericUpDown3.Value.ToString();
            trackBar3.Value = (int)numericUpDown3.Value;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
            numericUpDown1.Value = trackBar1.Value;
            
            textBox2.Text = trackBar2.Value.ToString();
            numericUpDown2.Value = trackBar2.Value;
            
            textBox3.Text = trackBar3.Value.ToString();
            numericUpDown3.Value = trackBar3.Value;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            trackBar1.Value = int.Parse(textBox1.Text);
            numericUpDown1.Value = decimal.Parse(textBox1.Text);
            
            trackBar2.Value = int.Parse(textBox2.Text);
            numericUpDown2.Value = decimal.Parse(textBox2.Text);
            
            trackBar3.Value = int.Parse(textBox3.Text);
            numericUpDown3.Value = decimal.Parse(textBox3.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = Properties.Settings.Default.Number1;
            numericUpDown2.Value = Properties.Settings.Default.Number2;
            numericUpDown3.Value = Properties.Settings.Default.Number3;

            
            numericUpDown_ValueChanged(null, null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Number1 = numericUpDown1.Value;
            Properties.Settings.Default.Number2 = numericUpDown2.Value;
            Properties.Settings.Default.Number3 = numericUpDown3.Value;

            Properties.Settings.Default.Save();
        }
    }
}
