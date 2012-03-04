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
            List<ITabbedDocument> matches = SearchUtil.getMatchedItems(PluginBase.MainForm.Documents, search, "\\", 0);
            foreach (ITabbedDocument document in matches)
            {
                string fileName = Path.GetFileName(document.FileName);
                string path = Path.GetDirectoryName(document.FileName);
                if (path.Length > 50)
                    path = "..." + path.Substring(path.Length - 47);
                ToolStripGroupItem foundItem = new ToolStripGroupItem(fileName, path, document.Icon.ToBitmap(), OpenDocument);
                foundItem.Tag = document;
                foundItems.Add(foundItem);
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

    #region Helpers

    class SearchUtil
    {
        public delegate Boolean Comparer(String value1, String value2, String value3);

        public static List<ITabbedDocument> getMatchedItems(IEnumerable<ITabbedDocument> source, String searchText, String pathSeparator, Int32 limit)
        {
            Int32 i = 0;
            List<ITabbedDocument> matchedItems = new List<ITabbedDocument>();
            String firstChar = searchText.Substring(0, 1);
            Comparer searchMatch = (firstChar == firstChar.ToUpper()) ? new Comparer(AdvancedSearchMatch) : new Comparer(SimpleSearchMatch);
            foreach (ITabbedDocument item in source)
            {
                if (searchMatch(item.FileName, searchText, pathSeparator))
                {
                    matchedItems.Add(item);
                    if (limit > 0 && i++ > limit) break;
                }
            }
            return matchedItems;
        }

        static private bool AdvancedSearchMatch(String file, String searchText, String pathSeparator)
        {
            int i = 0; int j = 0;
            if (file.Length < searchText.Length) return false;
            Char[] text = Path.GetFileName(file).ToCharArray();
            Char[] pattern = searchText.ToCharArray();
            while (i < pattern.Length)
            {
                while (i < pattern.Length && j < text.Length && pattern[i] == text[j])
                {
                    i++;
                    j++;
                }
                if (i == pattern.Length) return true;
                if (Char.IsLower(pattern[i])) return false;
                while (j < text.Length && Char.IsLower(text[j]))
                {
                    j++;
                }
                if (j == text.Length) return false;
                if (pattern[i] != text[j]) return false;
            }
            return (i == pattern.Length);
        }

        private static Boolean SimpleSearchMatch(String file, String searchText, String pathSeparator)
        {
            String fileName = Path.GetFileName(file).ToLower();
            return fileName.IndexOf(searchText.ToLower()) > -1;
        }

    }

    #endregion
}
