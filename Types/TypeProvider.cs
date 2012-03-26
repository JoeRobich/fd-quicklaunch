using System;
using System.Collections.Generic;
using System.Text;
using QuickLaunch.Managers;
using ASCompletion.Context;
using ASCompletion.Model;
using System.Windows.Forms;
using QuickLaunch.Controls;
using ASCompletion;

namespace QuickLaunch.Types
{
    public class TypeProvider : QuickLaunchProvider
    {
        public TypeProvider()
            : base("Types", "type")
        {
        }

        public override GroupItems FindItems(string search)
        {
            List<ToolStripGroupItem> foundItems = new List<ToolStripGroupItem>();
            List<MemberModel> matches = SearchUtil.getMatchedItems(ASContext.Context.GetAllProjectClasses(), search, 0);

            foreach (MemberModel member in matches)
            {
                int index = member.Name.LastIndexOf('.');
                string name = member.Name.Substring(index + 1);
                string packageName = index >= 0 ? member.Name.Substring(0, member.Name.Length - name.Length - 1) : string.Empty;

                ToolStripGroupItem foundItem = new ToolStripGroupItem(name, packageName, null, OpenType);
                foundItem.Tag = member;
                foundItems.Add(foundItem);
            }

            return new GroupItems(Name, foundItems);
        }

        private void OpenType(object sender, EventArgs e)
        {
            ToolStripGroupItem item = (ToolStripGroupItem)sender;
            MemberModel member = (MemberModel)item.Tag;
            
            int index = member.Name.LastIndexOf('.');
            string name = member.Name.Substring(index + 1);
            string packageName = index >= 0 ? member.Name.Substring(0, member.Name.Length - name.Length - 1) : string.Empty;
            
            ClassModel classModel = ASContext.Context.GetModel(packageName, name, packageName);
            if (!classModel.IsVoid() && classModel.InFile != null)
                ModelsExplorer.Instance.OpenFile(classModel.InFile.FileName);
        }
    }

    #region Helpers

    class SearchUtil
    {
        public delegate bool Comparer(string name, string search);

        public static List<MemberModel> getMatchedItems(MemberList source, string searchText, int limit)
        {
            int i = 0;
            List<MemberModel> matchedItems = new List<MemberModel>();
            char firstChar = searchText[0];
            Comparer searchMatch = char.IsUpper(firstChar) ? new Comparer(AdvancedSearchMatch) : new Comparer(SimpleSearchMatch);
            foreach (MemberModel item in source)
            {
                int index = item.Name.LastIndexOf('.');
                string name = item.Name.Substring(index + 1);

                if ((item.Flags & FlagType.Access) > 0 && searchMatch(name, searchText))
                {
                    matchedItems.Add(item);
                    if (limit > 0 && i++ > limit) break;
                }
            }
            return matchedItems;
        }

        static private bool AdvancedSearchMatch(String name, String searchText)
        {
            int i = 0; int j = 0;
            if (name.Length < searchText.Length) return false;
            char[] text = name.ToCharArray();
            char[] pattern = searchText.ToCharArray();
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

        private static bool SimpleSearchMatch(string name, string searchText)
        {
            return name.IndexOf(searchText,  StringComparison.OrdinalIgnoreCase) > -1;
        }

    }

    #endregion
}
