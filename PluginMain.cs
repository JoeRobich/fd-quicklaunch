using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore;
using System.Runtime.InteropServices;
using QuickLaunch.Controls;
using QuickLaunch.Managers;
using QuickLaunch.Menu;
using QuickLaunch.Documents;
using QuickLaunch.Types;
using QuickLaunch.Helpers;

namespace QuickLaunch
{
	public class PluginMain : IPlugin
	{
        private const int API = 1;
        private const string NAME = "QuickLaunch";
        private const string GUID = "128ACEAB-3A9A-48bf-8FEF-7A06475D02CF";
        private const string HELP = "www.flashdevelop.org/community/";
        private const string DESCRIPTION = "Adds a quick launch box that provides easy access to commands, files, classes, and more..";
        private const string AUTHOR = "Joey Robichaud";

        private string _settingFilename = "";
        private Settings _settings;
        private ToolStripSearchTextBox _quickLaunchBox = null;
        private ToolStripMenuItem _activateItem = null;

	    #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
		{
			get { return NAME; }
		}

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
		{
			get { return GUID; }
		}

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
		{
			get { return AUTHOR; }
		}

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
		{
			get { return DESCRIPTION; }
		}

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
		{
			get { return HELP; }
		}

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this._settings; }
        }
		
		#endregion
		
		#region Required Methods
		
		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public void Initialize()
		{
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();
            this.InitializeQuickLaunch();
            this.UpdateLocation();
            this.RegisterProviders();
        }
		
		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            this.SaveSettings();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{

		}

        void _settings_OnSettingsChanged()
        {
            UpdateLocation();
        }

        private void UpdateLocation()
        {
            RemoveQuickLaunch();
            AddQuickLaunch();
            UpdateWatermark();
        }

        private void AddQuickLaunch()
        {
            if (_settings.Location == QuickLaunchLocation.MenuStrip)
                PluginBase.MainForm.MenuStrip.Items.Add(_quickLaunchBox);
            else if (_settings.Location == QuickLaunchLocation.ToolStrip)
                PluginBase.MainForm.ToolStrip.Items.Add(_quickLaunchBox);
        }

        private void RemoveQuickLaunch()
        {
            if (PluginBase.MainForm.MenuStrip.Items.Contains(_quickLaunchBox))
                PluginBase.MainForm.MenuStrip.Items.Remove(_quickLaunchBox);

            if (PluginBase.MainForm.ToolStrip.Items.Contains(_quickLaunchBox))
                PluginBase.MainForm.ToolStrip.Items.Remove(_quickLaunchBox);
        }

        private void UpdateWatermark()
        {
            string watermark = ResourceHelper.GetString("QuickLaunch.Label.QuickLaunch");
            string shortcutString = string.Empty;

            if (!string.IsNullOrEmpty(_activateItem.ShortcutKeyDisplayString))
                shortcutString = _activateItem.ShortcutKeyDisplayString;
            else if (_activateItem.ShortcutKeys != Keys.None)
                shortcutString = TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(_activateItem.ShortcutKeys);

            if (!string.IsNullOrEmpty(shortcutString))
                watermark += " (" + shortcutString.Replace("Oemtilde", "~") + ")";

            _quickLaunchBox.WatermarkText = watermark;
        }
		
		#endregion

        #region Plugin Methods

        private void ActivateQuickLaunch(object sender, EventArgs e)
        {
            if (_settings.Location != QuickLaunchLocation.None)
                _quickLaunchBox.Focus();
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PluginCore.Helpers.PathHelper.DataDir, NAME);
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this._settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            // Set events you want to listen (combine as flags)
            EventManager.AddEventHandler(this, EventType.Command);
            ToolStripMenuItem menu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("SearchMenu");
            _activateItem = new ToolStripMenuItem(ResourceHelper.GetString("QuickLaunch.Label.ActivateQuickLaunch"), null, new EventHandler(ActivateQuickLaunch));
            _activateItem.Visible = false;
            _activateItem.ShortcutKeys = Keys.Control | Keys.Oemtilde;
            PluginBase.MainForm.RegisterShortcutItem("QuickLaunch.ActivateQuickLaunch", _activateItem);
            menu.DropDownItems.Add(_activateItem);
            _settings.OnSettingsChanged += new SettingsChangesEvent(_settings_OnSettingsChanged);
        }

        /// <summary>
        /// Adds shortcuts for manipulating the navigation bar
        /// </summary>
        public void InitializeQuickLaunch()
        {
            _quickLaunchBox = new ToolStripSearchTextBox();
            _quickLaunchBox.AutoSize = false;
            _quickLaunchBox.Width = 200;
            _quickLaunchBox.Font = PluginBase.Settings.DefaultFont;
            _quickLaunchBox.Alignment = ToolStripItemAlignment.Right;
            _quickLaunchBox.Search += new EventHandler(quickLaunchBox_Search);
        }

        void quickLaunchBox_Search(object sender, EventArgs e)
        {
            ToolStripSearchTextBox quickLaunchBox = (ToolStripSearchTextBox)sender;
            quickLaunchBox.Items.Clear();
            quickLaunchBox.Items.AddRange(QuickLaunchManager.FindItems(quickLaunchBox.Text));
            quickLaunchBox.DroppedDown = true;
        }

        public void RegisterProviders()
        {
            QuickLaunchManager.RegisterProvider(new MenuProvider());
            QuickLaunchManager.RegisterProvider(new DocumentProvider());
            QuickLaunchManager.RegisterProvider(new TypeProvider());
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this._settings = new Settings();
            if (!File.Exists(this._settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this._settingFilename, this._settings);
                this._settings = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this._settingFilename, this._settings);
        }

		#endregion
	}
}