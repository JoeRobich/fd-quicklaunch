using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (MemberModel member in ASContext.Context.GetAllProjectClasses())
            {
                int index = member.Name.LastIndexOf('.');
                string name = member.Name.Substring(index + 1);
                string packageName = index >= 0 ? member.Name.Substring(0, member.Name.Length - name.Length - 1) : string.Empty;
                
                if ((member.Flags & FlagType.Access) > 0 && 
                    name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ToolStripGroupItem foundItem = new ToolStripGroupItem(name, packageName, null, OpenType);
                    foundItem.Tag = member;
                    foundItems.Add(foundItem);
                }                
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
}
