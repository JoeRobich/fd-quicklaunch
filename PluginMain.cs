using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using QuickLaunch.Controls;
using QuickLaunch.Documents;
using QuickLaunch.Helpers;
using QuickLaunch.Managers;
using QuickLaunch.Menu;
using QuickLaunch.Types;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace QuickLaunch
{
    public class PluginMain : IPlugin
    {
        private const int API = 1;
        private const string NAME = "QuickLaunch";
        private const string GUID = "128ACEAB-3A9A-48bf-8FEF-7A06475D02CF";
        private const string HELP = "http://www.flashdevelop.org/community/viewtopic.php?f=4&t=9443";
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
        public int Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name
		{
			get { return NAME; }
		}

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid
		{
			get { return GUID; }
		}

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author
		{
			get { return AUTHOR; }
		}

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description
		{
			get { return DESCRIPTION; }
		}

        /// <summary>
        /// Web address for help
        /// </summary>
        public string Help
		{
			get { return HELP; }
		}

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings
        {
            get { return _settings; }
        }

		#endregion

		#region Required Methods

		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public void Initialize()
        {
            InitBasics();
            LoadSettings();
            AddEventHandlers();
            InitializeQuickLaunch();
            UpdateLocation();
            RegisterProviders();
        }

		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
        {
            SaveSettings();
		}

		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(object sender, NotifyEvent e, HandlingPriority prority)
		{
            if (e.Type == EventType.ApplyTheme)
            {
                _quickLaunchBox.ApplyTheme();
            }
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
            string dataPath = Path.Combine(PluginCore.Helpers.PathHelper.DataDir, NAME);
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            _settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            // Set events you want to listen (combine as flags)
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
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
            _quickLaunchBox.Name = ResourceHelper.GetString("QuickLaunch.Label.QuickLaunch");
            _quickLaunchBox.Renderer = new DockPanelStripRenderer();
            _quickLaunchBox.AutoSize = false;
            _quickLaunchBox.Width = ScaleHelper.Scale(200);
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
            _settings = new Settings();
            if (!File.Exists(_settingFilename)) SaveSettings();
            else
            {
                object obj = ObjectSerializer.Deserialize(_settingFilename, _settings);
                _settings = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(_settingFilename, _settings);
        }

		#endregion
	}
}