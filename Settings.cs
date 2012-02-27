using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using QuickLaunch.Localization;

namespace QuickLaunch
{
    public delegate void SettingsChangesEvent();

    [Serializable]
    public class Settings
    {
        [field: NonSerialized]
        public event SettingsChangesEvent OnSettingsChanged;

        private const QuickLaunchLocation DEFAULT_LOCATION = QuickLaunchLocation.MenuStrip;

        private QuickLaunchLocation _location = DEFAULT_LOCATION;

        [LocalizedCategory("QuickLaunch.Category.Visibility")]
        [LocalizedDisplayName("QuickLaunch.Label.Location")]
        [LocalizedDescription("QuickLaunch.Description.Location")]
        [DefaultValue(DEFAULT_LOCATION)]
        public QuickLaunchLocation Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    FireChanged();
                }
            }
        }

        private void FireChanged()
        {
            if (OnSettingsChanged != null) OnSettingsChanged();
        }
    }

    public enum QuickLaunchLocation
    {
        None,
        MenuStrip,
        ToolStrip
    }
}
