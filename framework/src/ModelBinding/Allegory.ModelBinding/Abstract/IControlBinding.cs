using System.Windows.Forms;

namespace Allegory.ModelBinding.Abstract
{
    public interface IControlBinding
    {
        void SetValue(Control control, object value);
        object GetValue(Control control);
    }
}
