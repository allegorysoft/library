using System.Reflection;

namespace Allegory.ModelBinding.Concrete
{
    public class BindingProperty
    {
        public string ControlName { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
