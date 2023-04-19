using System;
using System.Windows.Forms;
using Allegory.ModelBinding.Concrete;

namespace Allegory.ModelBinding.WinTests
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainModel model = new MainModel
            {
                textBox1 = "ahmetfarukulu",
                comboBox1 = 2,
                checkBox1 = true,
                dateTimePicker1 = new DateTime(2020, 1, 1),
                numericUpDown1 = 16,
                maskedTextBox1 = "21345642",
                Name = "John Doe"
            };
            ControlBindingExtension.GetFromModel(Controls, ref model);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainModel model = new MainModel();
            ControlBindingExtension.GetFromControl(Controls, ref model);
        }
    }
}
