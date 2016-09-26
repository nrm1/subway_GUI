using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace subway_GUI
{
    class GUI : InterfaceGUI
    {
        [STAThread]
        public int showGUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //Form Form1 = new Form();
            //Form1.Show();
            return 0;
        }
    }
}
