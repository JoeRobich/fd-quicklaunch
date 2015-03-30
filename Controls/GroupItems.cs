using System.Collections.Generic;
using System.Windows.Forms;

namespace QuickLaunch.Controls
{
    /// <summary>
    /// Contains a collection of ToolStripGroupItems.
    /// </summary>
    public class GroupItems
    {
        private string _name = string.Empty;
        private ToolStripItem[] _items = null;

        public GroupItems(string name, List<ToolStripGroupItem> items)
        {
            List<ToolStripItem> groupItems = new List<ToolStripItem>();

            // Add the group separator as the first item.
            groupItems.Add(new ToolStripGroupSeparator(string.Format("{0} ({1})", name, items.Count)));
            foreach (ToolStripItem item in items)
                groupItems.Add(item);
            _items = groupItems.ToArray();
        }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the items in the group.
        /// </summary>
        public ToolStripItem[] Items
        {
            get { return _items; }
        }
    }
}