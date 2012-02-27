using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickLaunch.Controls;

namespace QuickLaunch.Managers
{
    public abstract class QuickLaunchProvider
    {
        private string _name = string.Empty;
        private string _shortcut = string.Empty;

        public QuickLaunchProvider(string name, string shortcut)
        {
            _name = name;
            _shortcut = shortcut;
        }

        public string Name
        {
            get { return _name; }
        }

        public string ShortCut
        {
            get { return _shortcut; }
        }

        public abstract GroupItems FindItems(string search);
    }
}
