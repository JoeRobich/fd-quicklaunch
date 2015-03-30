using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using PluginCore.Helpers;

namespace QuickLaunch.Controls
{
    public class ToolStripGroupItem : ToolStripMenuItem
    {
        string _name = string.Empty;
        string _details = string.Empty;

        public ToolStripGroupItem(string name)
            : this(name, null, null, null)
        {

        }

        public ToolStripGroupItem(string name, string details)
            : this(name, details, null, null)
        {

        }

        public ToolStripGroupItem(string name, string details, Image image)
            : this(name, details, image, null)
        {

        }

        public ToolStripGroupItem(string name, string details, Image image, EventHandler onClick)
            : base(string.Format("{0} ({1})WWWWWW", name, details), image, onClick)
        {
            _name = name;
            _details = string.IsNullOrEmpty(details) ? string.Empty : "(" + details + ")";
        }

        protected bool IsVertical
        {
            get { return !IsOnOverflow && Owner.Orientation != Orientation.Horizontal; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!IsVertical)
            {
                // Draw the background
                Owner.Renderer.DrawMenuItemBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

                int padding = this.Owner.Padding.Left;

                // Draw the image
                padding += ScaleHelper.Scale(4);
                if (Image != null)
                {
                    Rectangle source = new Rectangle(0, 0, Image.Width, Image.Height);
                    Rectangle dest = new Rectangle(new Point(padding, 0), ScaleHelper.Scale(new Size(16, 16)));
                    dest.Offset(0, (e.ClipRectangle.Height - dest.Height) / 2);
                    e.Graphics.DrawImage(Image, dest, source, GraphicsUnit.Pixel);
                }

                // Draw the name text
                padding += ScaleHelper.Scale(23);
                Size textSize = TextRenderer.MeasureText(_name, Owner.Font);
                Rectangle textRectangle = new Rectangle(padding, 0, textSize.Width, this.Height);
                Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, _name, textRectangle, Owner.ForeColor, Owner.Font, ContentAlignment.MiddleLeft));

                // Draw the details text
                if (!string.IsNullOrEmpty(_details))
                {
                    padding += textSize.Width;
                    textSize = TextRenderer.MeasureText(_details, Owner.Font);
                    textRectangle = new Rectangle(padding, 0, textSize.Width, this.Height);
                    Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, _details, textRectangle, Color.Gray, Owner.Font, ContentAlignment.MiddleLeft));
                }
            }
            else
            {
                base.OnPaint(e);
            }
        }
    }
}
