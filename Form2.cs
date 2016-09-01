using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace External_Overlay
{
    public partial class Form2 : Form
    {
        List<Flask> Flasks = new List<Flask>();
        List<string> FlaskTypes = new List<string>();
        Form1 f;
        bool running = false;
        BackgroundWorker worker = new BackgroundWorker();
        public Form2()
        {
            Array fvalues = Enum.GetValues(typeof(Flask.Name));
            Array fvalues2 = Enum.GetValues(typeof(Flask.Name));
            Array fvalues3 = Enum.GetValues(typeof(Flask.Name));
            Array fvalues4 = Enum.GetValues(typeof(Flask.Name));
            Array fvalues5 = Enum.GetValues(typeof(Flask.Name));
            List<List<string>> valuesasdf = new List<List<string>>();
            Array values = Enum.GetValues(typeof(Keys2));
            Array values2 = Enum.GetValues(typeof(Keys2));
            Array values3 = Enum.GetValues(typeof(Keys2));
            Array values4 = Enum.GetValues(typeof(Keys2));
            Array values5 = Enum.GetValues(typeof(Keys2));
            InitializeComponent();
            this.comboBox1.DataSource = values;
            this.comboBox2.DataSource = values2;
            this.comboBox3.DataSource = values3;
            this.comboBox4.DataSource = values4;
            this.comboBox5.DataSource = values5;
            this.comboBox6.DataSource = fvalues;
            this.comboBox7.DataSource = fvalues2;
            this.comboBox8.DataSource = fvalues3;
            this.comboBox9.DataSource = fvalues4;
            this.comboBox10.DataSource = fvalues5;
            this.comboBox1.SelectedIndex = (int)Properties.Settings.Default["cb1"];
            this.comboBox2.SelectedIndex = (int)Properties.Settings.Default["cb2"];
            this.comboBox3.SelectedIndex = (int)Properties.Settings.Default["cb3"];
            this.comboBox4.SelectedIndex = (int)Properties.Settings.Default["cb4"];
            this.comboBox5.SelectedIndex = (int)Properties.Settings.Default["cb5"];
            this.comboBox6.SelectedIndex = (int)Properties.Settings.Default["cb6"];
            this.comboBox7.SelectedIndex = (int)Properties.Settings.Default["cb7"];
            this.comboBox8.SelectedIndex = (int)Properties.Settings.Default["cb8"];
            this.comboBox9.SelectedIndex = (int)Properties.Settings.Default["cb9"];
            this.comboBox10.SelectedIndex = (int)Properties.Settings.Default["cb10"];
            this.checkBox1.Checked = (bool)Properties.Settings.Default["check1"];
            this.checkBox2.Checked = (bool)Properties.Settings.Default["check2"];
            this.checkBox3.Checked = (bool)Properties.Settings.Default["check3"];
            this.checkBox4.Checked = (bool)Properties.Settings.Default["check4"];
            this.checkBox5.Checked = (bool)Properties.Settings.Default["check5"];
            this.checkBox6.Checked = (bool)Properties.Settings.Default["showWC"];
            this.numericUpDown1.Value = (int)Properties.Settings.Default["qual1"];
            this.numericUpDown2.Value = (int)Properties.Settings.Default["qual2"];
            this.numericUpDown3.Value = (int)Properties.Settings.Default["qual3"];
            this.numericUpDown4.Value = (int)Properties.Settings.Default["qual4"];
            this.numericUpDown5.Value = (int)Properties.Settings.Default["qual5"];
            this.numericUpDown6.Value = (int)Properties.Settings.Default["globalqual"];
            this.numericUpDown7.Value = (decimal)Properties.Settings.Default["Alpha"];
            this.numericUpDown8.Value = (int)Properties.Settings.Default["Xmovement"];
            this.numericUpDown9.Value = (int)Properties.Settings.Default["XResolution"];
            this.numericUpDown10.Value = (int)Properties.Settings.Default["YResolution"];
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                //saving settings
                Properties.Settings.Default["cb1"] = comboBox1.SelectedIndex;
                Properties.Settings.Default["cb2"] = comboBox2.SelectedIndex;
                Properties.Settings.Default["cb3"] = comboBox3.SelectedIndex;
                Properties.Settings.Default["cb4"] = comboBox4.SelectedIndex;
                Properties.Settings.Default["cb5"] = comboBox5.SelectedIndex;
                Properties.Settings.Default["cb6"] = comboBox6.SelectedIndex;
                Properties.Settings.Default["cb7"] = comboBox7.SelectedIndex;
                Properties.Settings.Default["cb8"] = comboBox8.SelectedIndex;
                Properties.Settings.Default["cb9"] = comboBox9.SelectedIndex;
                Properties.Settings.Default["cb10"] = comboBox10.SelectedIndex;
                Properties.Settings.Default["check1"] = checkBox1.Checked;
                Properties.Settings.Default["check2"] = checkBox2.Checked;
                Properties.Settings.Default["check3"] = checkBox3.Checked;
                Properties.Settings.Default["check4"] = checkBox4.Checked;
                Properties.Settings.Default["check5"] = checkBox5.Checked;
                Properties.Settings.Default["showWC"] = checkBox6.Checked;
                Properties.Settings.Default["qual1"] = (int)numericUpDown1.Value;
                Properties.Settings.Default["qual2"] = (int)numericUpDown2.Value;
                Properties.Settings.Default["qual3"] = (int)numericUpDown3.Value;
                Properties.Settings.Default["qual4"] = (int)numericUpDown4.Value;
                Properties.Settings.Default["qual5"] = (int)numericUpDown5.Value;
                Properties.Settings.Default["globalqual"] = (int)numericUpDown6.Value;
                Properties.Settings.Default["Alpha"] = (decimal)numericUpDown7.Value;
                Properties.Settings.Default["Xmovement"] = (int)numericUpDown8.Value;
                Properties.Settings.Default["XResolution"] = (int)numericUpDown9.Value;
                Properties.Settings.Default["YResolution"] = (int)numericUpDown10.Value;
                Properties.Settings.Default.Save();
                Flask f1 = new Flask(checkBox1.Checked, (Flask.Name)comboBox6.SelectedValue, (Keys2)comboBox1.SelectedItem, (int)numericUpDown1.Value);
                Flask f2 = new Flask(checkBox2.Checked, (Flask.Name)comboBox7.SelectedValue, (Keys2)comboBox2.SelectedItem, (int)numericUpDown2.Value);
                Flask f3 = new Flask(checkBox3.Checked, (Flask.Name)comboBox8.SelectedValue, (Keys2)comboBox3.SelectedItem, (int)numericUpDown3.Value);
                Flask f4 = new Flask(checkBox4.Checked, (Flask.Name)comboBox9.SelectedValue, (Keys2)comboBox4.SelectedItem, (int)numericUpDown4.Value);
                Flask f5 = new Flask(checkBox5.Checked, (Flask.Name)comboBox10.SelectedValue, (Keys2)comboBox5.SelectedItem, (int)numericUpDown5.Value);

                Flasks.Add(f1);
                Flasks.Add(f2);
                Flasks.Add(f3);
                Flasks.Add(f4);
                Flasks.Add(f5);
                f = new Form1(Flasks, (int)numericUpDown6.Value, (float)numericUpDown7.Value, (int)numericUpDown8.Value, (float)numericUpDown9.Value, (float)numericUpDown10.Value, checkBox6.Checked);
                f.Show();
                running = true;

               
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            f.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (f != null)
            {
                f.Close();
                f = new Form1(Flasks, (int)numericUpDown6.Value, (float)numericUpDown7.Value, (int)numericUpDown8.Value, (float)numericUpDown9.Value, (float)numericUpDown10.Value, checkBox6.Checked);
                f.Show();
            }
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
