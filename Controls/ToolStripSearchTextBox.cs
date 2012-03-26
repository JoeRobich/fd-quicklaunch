using System;
using System.Collections.Generic;
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
            // Clear the search textbox when the icon is clicked.
            _searchIcon.Click += new EventHandler(_searchIcon_Click);
            TextBox.Controls.Add(_searchIcon);

            // Update the icon in the search box
            TextBox.TextChanged += new EventHandler(TextBox_TextChanged);
        }

        public event EventHandler Search;

        /// <summary>
        /// Gets the items collection for the search dropdown.
        /// </summary>
        public ToolStripItemCollection Items
        {
            get { return _dropdown.Items; }
        }

        /// <summary>
        /// Gets/Sets the text for the watermark.
        /// </summary>
        public string WatermarkText
        {
            get { return _watermarkText; }
            set { _watermarkText = value; ApplyWatermark(); }
        }

        /// <summary>
        /// Gets/Sets whether the search dropdown is displayed.
        /// </summary>
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

        /// <summary>
        /// Gets/Sets the render to use for the search dropdown.
        /// </summary>
        public ToolStripRenderer Renderer
        {
            get { return _dropdown.Renderer; }
            set { _dropdown.Renderer = value; }
        }

        private void ApplyWatermark()
        {
            TextBoxHelper.SetWatermark(TextBox, _watermarkText);
            TextBoxHelper.SetRightMargin(TextBox, 23);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            // If enter then fire the Search event
            if (keyData == Keys.Enter)
            {
                OnSearch();
                return true;
            }
            // If control then check from a editing shortcut
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
            // If the text length is 0 then show a magnifier, otherwise show a x
            _searchIcon.Image = TextBox.TextLength == 0 ? PluginBase.MainForm.FindImage("251") : PluginBase.MainForm.FindImage("153");
        }

        void _searchIcon_Click(object sender, EventArgs e)
        {
            TextBox.Text = "";
        }
    }

    internal static class TextBoxHelper
    {
        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;
        private const uint EM_SETMARGIN = 0xd3;
        private const uint EC_RIGHTMARGIN = 2;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, IntPtr lParam);

        /// <summary>
        /// Sets the watermark for a textbox
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="watermarkText"></param>
        public static void SetWatermark(TextBox textBox, string watermarkText)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermarkText);
        }

        /// <summary>
        /// Sets the interior right margin for a textbox
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="margin"></param>
        public static void SetRightMargin(TextBox textBox, int margin)
        {
            SendMessage(textBox.Handle, EM_SETMARGIN, EC_RIGHTMARGIN, (IntPtr)(margin << 16));
        }
    }
}
