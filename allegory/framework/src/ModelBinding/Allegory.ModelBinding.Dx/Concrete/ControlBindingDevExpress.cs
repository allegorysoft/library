using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Allegory.ModelBinding.Abstract;
using Allegory.ModelBinding.Concrete;
using DevExpress.XtraEditors;

namespace Allegory.ModelBinding.Dx.Concrete
{
    public class ControlBindingDevExpress : ControlBindingBase
    {
        public ControlBindingDevExpress()
        {
            SupportedControl = new HashSet<Type>
            {
                typeof(BaseEdit),
                //etc...
            };
        }

        public override object GetValue(Control control)
        {
            if (control is ComboBoxEdit && control.Tag?.ToString() == "Index")
                return ((ComboBoxEdit)control).SelectedIndex;

            if (control is BaseEdit)
                return ((BaseEdit)control).EditValue;

            throw new ControlBindingException("Invalid control type");
        }
        public override void SetValue(Control control, object value)
        {
            if (control is ComboBoxEdit && control.Tag?.ToString() == "Index")
                ((ComboBoxEdit)control).SelectedIndex = Convert.ToInt32(value);

            else if (control is BaseEdit)
                ((BaseEdit)control).EditValue = value;

            else
                throw new ControlBindingException("Invalid control type");
        }
    }
}
