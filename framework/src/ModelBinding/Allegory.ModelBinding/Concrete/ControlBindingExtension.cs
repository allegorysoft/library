using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Allegory.ModelBinding.Abstract;

namespace Allegory.ModelBinding.Concrete
{
    public static class ControlBindingExtension
    {
        private static HashSet<ControlBindingBase> _controlBindings;

        public static void SetControlBindings(HashSet<ControlBindingBase> controlBindings) => _controlBindings = controlBindings;

        public static object GetValue(this Control control)
        {
            var controlBinding = _controlBindings.FirstOrDefault(f => f.IsControlSupported(control));

            if (controlBinding == null)
                throw new ControlBindingException("Invalid control type");
            else
                return controlBinding.GetValue(control);
        }
        public static void SetValue(this Control control, object value)
        {
            var controlBinding = _controlBindings.FirstOrDefault(f => f.IsControlSupported(control));

            if (controlBinding == null)
                throw new ControlBindingException("Invalid control type");
            else
                controlBinding.SetValue(control, value);
        }

        private static List<BindingProperty> GetBindingProperties<T>()
        {
            return typeof(T).GetProperties()
                             .Select(property => new BindingProperty
                             {
                                 ControlName = property.GetCustomAttribute(typeof(ModelInfoAttribute)) == null
                                             ? property.Name : (property.GetCustomAttribute(typeof(ModelInfoAttribute)) as ModelInfoAttribute).ControlName,
                                 Property = property
                             })
                             .ToList();
        }

        public static void GetFromControl<T>(this Control.ControlCollection controls, ref T model, bool clearGarbage = true)
        {
            GetFromControl(controls, ref model, GetBindingProperties<T>());

            if (clearGarbage)
                GC.Collect();
        }
        private static void GetFromControl<T>(this Control.ControlCollection controls, ref T model, List<BindingProperty> bindingProperties)
        {
            for (int i = 0; i < controls.Count && bindingProperties.Count > 0; i++)
            {
                var property = bindingProperties.FirstOrDefault(p => p.ControlName == controls[i].Name);
                if (property != null)
                {
                    object value = controls[i].GetValue();
                    property.Property.SetValue(
                        model,
                        value == null
                              ? null
                              : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.Property.PropertyType) ?? property.Property.PropertyType)
                      );
                    bindingProperties.Remove(property);
                }
                else if (controls[i].Controls.Count > 0)
                    controls[i].Controls.GetFromControl(ref model, bindingProperties);
            }
        }

        public static void GetFromModel<T>(this Control.ControlCollection controls, ref T model, bool clearGarbage = true)
        {
            GetFromModel(controls, ref model, GetBindingProperties<T>());

            if (clearGarbage)
                GC.Collect();
        }
        private static void GetFromModel<T>(this Control.ControlCollection controls, ref T model, List<BindingProperty> bindingProperties)
        {
            for (int i = 0; i < controls.Count && bindingProperties.Count > 0; i++)
            {
                var property = bindingProperties.FirstOrDefault(p => p.ControlName == controls[i].Name);
                if (property != null)
                {
                    controls[i].SetValue(property.Property.GetValue(model));
                    bindingProperties.Remove(property);
                }
                else if (controls[i].Controls.Count > 0)
                    controls[i].Controls.GetFromModel(ref model, bindingProperties);
            }
        }
    }
}
