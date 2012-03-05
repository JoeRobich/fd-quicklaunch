using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickLaunch.Controls;

namespace QuickLaunch.Managers
{
    /// <summary>
    /// An abstract base class that is extended to create new QuickLaunch Providers.
    /// </summary>
    public abstract class QuickLaunchProvider
    {
        private string _name = string.Empty;
        private string _shortcut = string.Empty;

        /// <summary>
        /// Constructs a new QuickLaunchProvider
        /// </summary>
        /// <param name="name">The name of this provider.</param>
        /// <param name="shortcut">The shortcut for this provider.</param>
        public QuickLaunchProvider(string name, string shortcut)
        {
            _name = name;
            _shortcut = shortcut;
        }

        /// <summary>
        /// Gets the name of this provider.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the shortcut for this provider.
        /// </summary>
        public string ShortCut
        {
            get { return _shortcut; }
        }

        /// <summary>
        /// Finds the items within this provider than matches the search string.
        /// </summary>
        /// <param name="search">The text to search for.</param>
        /// <returns>A group of items that match the search string.</returns>
        public abstract GroupItems FindItems(string search);
    }
}
