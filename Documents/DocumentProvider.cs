using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickLaunch.Managers;
using PluginCore;
using System.Windows.Forms;
using QuickLaunch.Controls;
using System.IO;

namespace QuickLaunch.Documents
{
    public class DocumentProvider : QuickLaunchProvider
    {
        public DocumentProvider()
            : base("Documents", "doc")
        {
        }

        public override GroupItems FindItems(string search)
        {
            List<ToolStripGroupItem> foundItems = new List<ToolStripGroupItem>();

            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                if (document.FileName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string fileName = Path.GetFileName(document.FileName);
                    string path = Path.GetDirectoryName(document.FileName);
                    if (path.Length > 50)
                        path = "..." + path.Substring(path.Length - 47);
                    ToolStripGroupItem foundItem = new ToolStripGroupItem(fileName, path, document.Icon.ToBitmap(), OpenDocument);
                    foundItem.Tag = document;
                    foundItems.Add(foundItem);
                }
            }

            return new GroupItems(Name, foundItems);
        }

        private void OpenDocument(object sender, EventArgs e)
        {
            ToolStripGroupItem item = (ToolStripGroupItem)sender;
            ITabbedDocument document = (ITabbedDocument)item.Tag;
            document.Activate();
        }
    }
}
