using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace QuickLaunch.Controls
{
    public class ToolStripGroupSeparator : ToolStripSeparator
    {
        public ToolStripGroupSeparator(string name)
        {
            Name = name;
        }

        protected bool IsVertical
        {
            get { return !IsOnOverflow && Owner.Orientation != Orientation.Horizontal; }
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = base.GetPreferredSize(constrainingSize);

            if (!IsVertical)
            {
                preferredSize.Height = 23;
            }

            return preferredSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!IsVertical)
            {
                Size textSize = TextRenderer.MeasureText(Name, Owner.Font);
                Rectangle textRectangle = new Rectangle(Owner.Padding.Left - 2, 0, textSize.Width + 2 + Owner.Padding.Left, this.Height);

                Region oldClip = e.Graphics.Clip;
                e.Graphics.Clip = new Region(new Rectangle(textRectangle.Width + e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - (textRectangle.Width + e.ClipRectangle.X), e.ClipRectangle.Height));
                base.OnPaint(e);
                e.Graphics.Clip = oldClip;

                Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Name, textRectangle, Color.Blue, Owner.Font, ContentAlignment.MiddleLeft));
            }
            else
            {
                base.OnPaint(e);
            }
        }
    }
}
