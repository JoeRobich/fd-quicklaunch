using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace QuickLaunch.Controls
{
    public class ToolStripSearchTextBox : ToolStripTextBox
    {
        private string _watermarkText = "";
        private ToolStripDropDownMenu _dropdown = null;

        public ToolStripSearchTextBox()
        {
            this.AcceptsReturn = true;
            _dropdown = new ToolStripDropDownMenu();
            _dropdown.ShowCheckMargin = false;
            _dropdown.ShowImageMargin = false;
            _dropdown.ItemClicked += new ToolStripItemClickedEventHandler(_dropdown_ItemClicked);
        }

        public event EventHandler Search;

        public ToolStripItemCollection Items
        {
            get { return _dropdown.Items; }
        }

        public string WatermarkText
        {
            get { return _watermarkText; }
            set { _watermarkText = value; ApplyWatermark(); }
        }

        public bool DroppedDown
        {
            get
            {
                return _dropdown.Visible;
            }

            set
            {
                if (value && !DroppedDown)
                    _dropdown.Show(TextBox, new Point(TextBox.Width, TextBox.Height), ToolStripDropDownDirection.BelowLeft);
                else if (!value && DroppedDown)
                    _dropdown.Hide();
            }
        }

        private void ApplyWatermark()
        {
            TextBox.SetWatermark(_watermarkText);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                OnSearch();
                return true;
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        protected void OnSearch()
        {
            if (Search != null)
                Search(this, new EventArgs());
        }

        void _dropdown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.Text = "";
        }
    }

    internal static class TextBoxWatermarkExtensionMethod
    {
        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        public static void SetWatermark(this TextBox textBox, string watermarkText)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermarkText);
        }
    }
}
