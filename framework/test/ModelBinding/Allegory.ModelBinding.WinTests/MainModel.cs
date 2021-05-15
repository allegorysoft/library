using System;
using Allegory.ModelBinding.Concrete;

namespace Allegory.ModelBinding.WinTests
{
    class MainModel
    {
        public string textBox1 { get; set; }
        public string maskedTextBox1 { get; set; }
        public DateTime dateTimePicker1 { get; set; }
        public decimal numericUpDown1 { get; set; }
        public int comboBox1 { get; set; }
        public bool checkBox1 { get; set; }
        [ModelInfo("textBox2")]
        public string Name { get; set; }
    }
}
