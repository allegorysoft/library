using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Allegory.ModelBinding.Abstract;
using Allegory.ModelBinding.Concrete;
using Allegory.ModelBinding.Dx.Concrete;

namespace Allegory.ModelBinding.DxTests
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ControlBindingExtension.SetControlBindings(new HashSet<ControlBindingBase>
            {
                new ControlBindingWin(),
                new ControlBindingDevExpress()
            });
            Application.Run(new FrmMain());
        }
    }
}
