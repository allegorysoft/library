using System;

namespace Allegory.ModelBinding.Concrete
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModelInfoAttribute : Attribute
    {
        public readonly string ControlName;
        public ModelInfoAttribute(string controlName)
        {
            ControlName = controlName;
        }
    }
}
