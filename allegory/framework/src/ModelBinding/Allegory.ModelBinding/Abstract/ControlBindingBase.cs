using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Allegory.ModelBinding.Abstract
{
    public abstract class ControlBindingBase : IControlBinding
    {
        public HashSet<Type> SupportedControl { get; protected set; }

        public abstract object GetValue(Control control);
        public abstract void SetValue(Control control, object value);

        public bool IsControlSupported(Control control)
        {
            bool controlSupport = SupportedControl.Contains(control.GetType());

            if (controlSupport)
                return true;
            else
            {
                foreach (var supportedControl in SupportedControl)
                {
                    controlSupport = supportedControl.IsAssignableFrom(control.GetType());
                    if (controlSupport)
                    {
                        SupportedControl.Add(control.GetType());
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
