using PluginCore;
using QuickLaunch.Controls;
using QuickLaunch.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuickLaunch.Menu
{
    public class MenuProvider : QuickLaunchProvider
    {
        public MenuProvider()
            : base("Commands", "cmd")
        {

        }

        public override GroupItems FindItems(string search)
        {
            return new GroupItems(this.Name, FindItems(search, PluginBase.MainForm.MenuStrip.Items, string.Empty));
        }

        private List<ToolStripGroupItem> FindItems(string search, ToolStripItemCollection items, string path)
        {
            List<ToolStripGroupItem> foundItems = new List<ToolStripGroupItem>();

            foreach (ToolStripItem stripItem in items)
            {
                ToolStripMenuItem item = stripItem as ToolStripMenuItem;
                if (item == null)
                    continue;

                if (item.Name == "ReopenMenu")
                    continue;

                string itemText = item.Text.Replace("&", "");

                if (item.HasDropDownItems)
                {
                    foundItems.AddRange(FindItems(search, item.DropDownItems, path + itemText + " → "));
                }
                else
                {
                    if (itemText.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ToolStripGroupItem foundItem;

                        string shortcutString = string.Empty;

                        if (!string.IsNullOrEmpty(item.ShortcutKeyDisplayString))
                            shortcutString = item.ShortcutKeyDisplayString;
                        else if (item.ShortcutKeys != Keys.None)
                            shortcutString = TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(item.ShortcutKeys);

                        foundItem = new ToolStripGroupItem(string.Format("{0}{1}", path, itemText), shortcutString, item.Image, ClickItem);
                        foundItem.Tag = item;
                        foundItems.Add(foundItem);
                    }
                }
            }

            return foundItems;
        }

        private void ClickItem(object sender, EventArgs e)
        {
            ToolStripGroupItem item = (ToolStripGroupItem)sender;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)item.Tag;
            menuItem.PerformClick();
        }
    }
}