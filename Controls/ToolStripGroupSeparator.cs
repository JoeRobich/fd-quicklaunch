using System;
using System.Collections.Generic;
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

            // If the separator is vertical
            if (!IsVertical)
            {
                // Ensure it's height matches that of the menu items, since we are drawing a label
                preferredSize.Height = 23;
            }

            return preferredSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!IsVertical)
            {
                // Determine the rectangle for the label
                Size textSize = TextRenderer.MeasureText(Name, Owner.Font);
                Rectangle textRectangle = new Rectangle(Owner.Padding.Left - 2, 0, textSize.Width + 2 + Owner.Padding.Left, this.Height);

                // Save the clip rectangle
                Region oldClip = e.Graphics.Clip;

                // Remove the area for the label from the clip region and draw the separator
                e.Graphics.Clip = new Region(new Rectangle(textRectangle.Width + e.ClipRectangle.X, e.ClipRectangle.Y, Owner.Width - textRectangle.Right - Owner.Padding.Right, e.ClipRectangle.Height));
                base.OnPaint(e);

                // Reset the clip region
                e.Graphics.Clip = oldClip;

                // Draw the group name
                Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Name, textRectangle, Color.Blue, Owner.Font, ContentAlignment.MiddleLeft));
            }
            else
            {
                base.OnPaint(e);
            }
        }
    }
}
