using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Solitaire
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
        }

        public static EventHandler TickUpdateEvents;
        private void TickUpdate(object sender, EventArgs e)
        {
            if (TickUpdateEvents == null) return;
            TickUpdateEvents.Invoke(sender, e);
        }
    }
}
