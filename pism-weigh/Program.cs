using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using pism_weigh.Database;

namespace pism_weigh
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DatabaseHelper.Initialize();
            Application.Run(new Form1());
        }
    }
}
