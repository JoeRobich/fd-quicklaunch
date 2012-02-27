using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickLaunch.Controls;

namespace QuickLaunch.Managers
{
    public class QuickLaunchManager
    {
        private static Dictionary<string, QuickLaunchProvider> _providerMap = new Dictionary<string, QuickLaunchProvider>();

        public static void RegisterProvider(QuickLaunchProvider provider)
        {
            _providerMap[provider.ShortCut] = provider;
        }

        public static void UnregisterProvider(QuickLaunchProvider provider)
        {
            UnregisterProvider(provider.Name);
        }

        public static void UnregisterProvider(string name)
        {
            if (_providerMap.ContainsKey(name))
                _providerMap.Remove(name);
        }

        public static ToolStripItem[] FindItems(string search)
        {
            List<ToolStripItem> foundItems = new List<ToolStripItem>();
            search = search.TrimStart();

            if (search.StartsWith("@"))
            {
                int index = search.IndexOf(' ');

                if (index < 0)
                    return foundItems.ToArray();

                string shortcut = search.Substring(0, search.IndexOf(' ')).TrimStart('@');
                search = search.Substring(shortcut.Length + 2).TrimStart();

                if (search.Length == 0)
                    return foundItems.ToArray();

                foreach (QuickLaunchProvider provider in _providerMap.Values)
                {
                    if (string.Equals(provider.ShortCut, shortcut, StringComparison.OrdinalIgnoreCase))
                    {
                        GroupItems items = provider.FindItems(search);
                        if (items.Items.Length > 1)
                            foundItems.AddRange(items.Items);
                        break;
                    }
                }       
            }
            else
            {
                foreach (QuickLaunchProvider provider in _providerMap.Values)
                {
                    GroupItems items = provider.FindItems(search);
                    if (items.Items.Length > 1)
                        if (items.Items.Length <= 11)
                            foundItems.AddRange(items.Items);
                        else
                        {
                            ((ToolStripGroupSeparator)items.Items[0]).Name += "*";
                            for (int index = 0; index < 11; index++)
                                foundItems.Add(items.Items[index]);
                        }
                }
            }

            return foundItems.ToArray();
        }
    }
}
