using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Allegory.ModelBinding.Abstract;

namespace Allegory.ModelBinding.Concrete
{
    public class ControlBindingWin : ControlBindingBase
    {
        public ControlBindingWin()
        {
            SupportedControl = new HashSet<Type>()
            {
                typeof(TextBoxBase),
                typeof(DateTimePicker),
                typeof(NumericUpDown),
                typeof(ComboBox),
                typeof(CheckBox),
            };
        }

        public override object GetValue(Control control)
        {
            if (control is TextBoxBase)
                return control.Text == string.Empty ? null : control.Text;

            if (control is DateTimePicker)
                return ((DateTimePicker)control).Value;

            if (control is NumericUpDown)
                return ((NumericUpDown)control).Value;

            if (control is ComboBox)
                return ((ComboBox)control).ValueMember != string.Empty
                       ? ((ComboBox)control).SelectedValue : ((ComboBox)control).SelectedIndex;

            if (control is CheckBox)
                return ((CheckBox)control).Checked;

            throw new ControlBindingException("Invalid control type");
        }
        public override void SetValue(Control control, object value)
        {
            if (control is TextBoxBase)
                control.Text = (string)value;

            else if (control is DateTimePicker)
                ((DateTimePicker)control).Value = Convert.ToDateTime(value);

            else if (control is NumericUpDown)
                ((NumericUpDown)control).Value = Convert.ToDecimal(value);

            else if (control is ComboBox)
                if (((ComboBox)control).ValueMember != string.Empty)
                    ((ComboBox)control).SelectedValue = value;
                else
                    ((ComboBox)control).SelectedIndex = Convert.ToInt32(value);

            else if (control is CheckBox)
                ((CheckBox)control).Checked = Convert.ToBoolean(value);

            else
                throw new ControlBindingException("Invalid control type");
        }
    }
}
