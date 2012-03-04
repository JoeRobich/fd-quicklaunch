using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using QuickLaunch.Helpers;
using PluginCore;

namespace QuickLaunch.Controls
{
    public class ToolStripSearchTextBox : ToolStripTextBox
    {
        private string _watermarkText = "";
        private ToolStripDropDownMenu _dropdown = null;
        private PictureBox _searchIcon = new PictureBox();

        public ToolStripSearchTextBox()
        {
            this.AcceptsReturn = true;
            TextBox.ShortcutsEnabled = true;
            _dropdown = new ToolStripDropDownMenu();
            _dropdown.ShowCheckMargin = false;
            _dropdown.ShowImageMargin = false;
            _dropdown.ItemClicked += new ToolStripItemClickedEventHandler(_dropdown_ItemClicked);

            _searchIcon = new PictureBox();
            _searchIcon.Location = new Point(179, 2);
            _searchIcon.Size = new Size(23, 23);
            _searchIcon.Image = PluginBase.MainForm.FindImage("251");
            _searchIcon.Cursor = Cursors.Default;
            _searchIcon.Click += new EventHandler(_searchIcon_Click);
            TextBox.Controls.Add(_searchIcon);

            TextBox.TextChanged += new EventHandler(TextBox_TextChanged);
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

        public ToolStripRenderer Renderer
        {
            get { return _dropdown.Renderer; }
            set { _dropdown.Renderer = value; }
        }

        private void ApplyWatermark()
        {
            TextBox.SetWatermark(_watermarkText);
            TextBox.SetRightMargin(23);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                OnSearch();
                return true;
            }
            else if ((keyData & Keys.Control) == Keys.Control)
            {
                if ((keyData & Keys.V) == Keys.V)
                {
                    TextBox.Paste();
                    return true;
                }
                else if ((keyData & Keys.C) == Keys.C)
                {
                    TextBox.Copy();
                    return true;
                }
                else if ((keyData & Keys.A) == Keys.A)
                {
                    TextBox.SelectAll();
                    return true;
                }
                else if ((keyData & Keys.X) == Keys.X)
                {
                    TextBox.Cut();
                    return true;
                }
                else if ((keyData & Keys.Z) == Keys.Z)
                {
                    TextBox.Undo();
                    return true;
                }
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

        void TextBox_TextChanged(object sender, EventArgs e)
        {
            _searchIcon.Image = TextBox.TextLength == 0 ? PluginBase.MainForm.FindImage("251") : PluginBase.MainForm.FindImage("153");
        }

        void _searchIcon_Click(object sender, EventArgs e)
        {
            TextBox.Text = "";
        }
    }

    internal static class TextBoxWatermarkExtensionMethod
    {
        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;
        private const uint EM_SETMARGIN = 0xd3;
        private const uint EC_RIGHTMARGIN = 2;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, IntPtr lParam);

        public static void SetWatermark(this TextBox textBox, string watermarkText)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermarkText);
        }

        public static void SetRightMargin(this TextBox textBox, int margin)
        {
            SendMessage(textBox.Handle, EM_SETMARGIN, EC_RIGHTMARGIN, (IntPtr)(margin << 16));
        }
    }
}
