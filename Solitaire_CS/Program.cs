using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Solitaire
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();

            Solitaire solitaire = new Solitaire();
            form.Controls.Add(solitaire.formPanel);
            form.SizeChanged += new EventHandler(solitaire.SizeChanged);

            Application.Run(form);
        }
    }
}
