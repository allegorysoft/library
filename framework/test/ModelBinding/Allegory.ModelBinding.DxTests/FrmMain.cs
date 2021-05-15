using System;
using System.Data;
using Allegory.ModelBinding.Concrete;
using DevExpress.XtraEditors;

namespace Allegory.ModelBinding.DxTests
{
    public partial class FrmMain : XtraForm
    {
        public FrmMain()
        {
            InitializeComponent();
            DataTable dt = new DataTable();
            dt.Columns.Add("NR", typeof(int));
            dt.Columns.Add("NAME");
            dt.Rows.Add(1, "LueItem1");
            dt.Rows.Add(2, "LueItem2");
            dt.Rows.Add(3, "LueItem3");
            lookUpEdit1.Properties.DataSource = dt;
            lookUpEdit1.Properties.DisplayMember = "NAME";
            lookUpEdit1.Properties.ValueMember = "NR";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            MainModel model = new MainModel
            {
                textEdit1 = "sample",
                buttonEdit1 = "butonedt",
                checkedComboBoxEdit1 = "Item1, Item3",
                comboBoxEdit1 = 1,
                comboBoxEdit2 = "ShowItemFromText2"
            };
            ControlBindingExtension.GetFromModel(Controls, ref model);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            MainModel model = new MainModel();
            ControlBindingExtension.GetFromControl(Controls, ref model);
        }
    }
}
