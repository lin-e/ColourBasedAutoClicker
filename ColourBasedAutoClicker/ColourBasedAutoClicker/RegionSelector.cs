using System;
using System.Drawing;
using System.Windows.Forms;

namespace ColourBasedAutoClicker
{
    public partial class RegionSelector : Form
    {
        public RegionSelector()
        {
            TransparencyKey = Color.Turquoise;
            BackColor = Color.Turquoise;
            InitializeComponent();
        }
        private void RegionSelector_Load(object sender, EventArgs e)
        {
            Text = string.Empty;
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
        }
    }
}
