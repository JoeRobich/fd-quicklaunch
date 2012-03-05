using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickLaunch.Controls;

namespace QuickLaunch.Managers
{
    /// <summary>
    /// Manages the QuickLaunch Providers
    /// </summary>
    public class QuickLaunchManager
    {
        private static Dictionary<string, QuickLaunchProvider> _providerMap = new Dictionary<string, QuickLaunchProvider>();

        /// <summary>
        /// Adds a Provider to be searched from the quick launch bar.
        /// </summary>
        /// <param name="provider"></param>
        public static void RegisterProvider(QuickLaunchProvider provider)
        {
            _providerMap[provider.ShortCut] = provider;
        }

        /// <summary>
        /// Removes a Provider from being searched from the quick launch bar.
        /// </summary>
        /// <param name="provider">The provider to unregister.</param>
        public static void UnregisterProvider(QuickLaunchProvider provider)
        {
            UnregisterProvider(provider.Name);
        }

        /// <summary>
        /// Removes a Provider from being searched from the quick launch bar.
        /// </summary>
        /// <param name="name">The Name of the provider to unregister.</param>
        public static void UnregisterProvider(string name)
        {
            if (_providerMap.ContainsKey(name))
                _providerMap.Remove(name);
        }

        /// <summary>
        /// Searches all the QuickLaunch Providers for the specified string.
        /// </summary>
        /// <param name="search">The text to search for.</param>
        /// <returns>The items to populate the QuickLaunch dropdown with.</returns>
        public static ToolStripItem[] FindItems(string search)
        {
            List<ToolStripItem> foundItems = new List<ToolStripItem>();
            search = search.TrimStart();

            // Are we searching with a shortcut?
            if (search.StartsWith("@"))
            {
                // Search a single provider and show all the results.
                int index = search.IndexOf(' ');

                // If there is no search string the return an empty array.
                if (index < 0)
                    return foundItems.ToArray();

                // Separate the shortcut from the search string.
                string shortcut = search.Substring(0, search.IndexOf(' ')).TrimStart('@');
                search = search.Substring(shortcut.Length + 2).TrimStart();

                // If the search string is empty then return an empty array
                if (search.Length == 0)
                    return foundItems.ToArray();

                // Find the provider with the matching shortcut
                foreach (QuickLaunchProvider provider in _providerMap.Values)
                {
                    // If the shortcuts match
                    if (string.Equals(provider.ShortCut, shortcut, StringComparison.OrdinalIgnoreCase))
                    {
                        // Return all the found items
                        GroupItems items = provider.FindItems(search);
                        if (items.Items.Length > 1)
                            foundItems.AddRange(items.Items);
                        break;
                    }
                }       
            }
            else
            {
                // Search each provider and only use the first 10 results. Query a particular provider to 
                // return all the results.
                foreach (QuickLaunchProvider provider in _providerMap.Values)
                {
                    GroupItems items = provider.FindItems(search);
                    if (items.Items.Length > 1)
                        // If less that 11 results
                        if (items.Items.Length <= 11)
                            // Add all the results
                            foundItems.AddRange(items.Items);
                        else
                        {
                            // More than 10 results so signify incomplete results with an asterisk
                            ((ToolStripGroupSeparator)items.Items[0]).Name += "*";
                            for (int index = 0; index < 11; index++)
                                // Add the first 10 items
                                foundItems.Add(items.Items[index]);
                        }
                }
            }

            return foundItems.ToArray();
        }
    }
}
