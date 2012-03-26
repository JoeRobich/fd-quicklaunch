﻿using System;
using System.Collections.Generic;
using System.Text;
using PluginCore.Localization;
using PluginCore;
using System.Resources;
using System.Reflection;
using System.Drawing;

namespace QuickLaunch.Helpers
{
    public class ResourceHelper
    {
        private static ResourceManager resourceManager = null;
        private static LocaleVersion storedLocale = LocaleVersion.en_US;
        private static Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();

        /// <summary>
        /// Gets the specified localized string.
        /// </summary>
        public static string GetString(string key)
        {
            string result;
            LocaleVersion localeSetting = PluginBase.MainForm.Settings.LocaleVersion;
            if (resourceManager == null || localeSetting != storedLocale)
            {
                storedLocale = localeSetting;
                Assembly callingAssembly = Assembly.GetCallingAssembly();
                string prefix = callingAssembly.GetName().Name;
                string path = prefix + ".Resources." + storedLocale.ToString();
                resourceManager = new ResourceManager(path, callingAssembly);
            }
            result = resourceManager.GetString(key);
            if (result == null)
                result = key;
            return result;
        }

        /// <summary>
        /// Gets the embedded image from the Resources folder. Assumes that the image is a png.
        /// </summary>
        /// <param name="name">The name of the embedded image</param>
        /// <returns></returns>
        public static Image GetImage(string name)
        {
            return GetImage(name, "png");
        }

        /// <summary>
        /// Gets the embedded image from the Resources folder.
        /// </summary>
        /// <param name="name">The name of the image.</param>
        /// <param name="extension">The extension of the image.</param>
        /// <returns></returns>
        public static Image GetImage(string name, string extension)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            string prefix = callingAssembly.GetName().Name;
            string resourceName = string.Format("{0}.Resources.{1}.{2}", prefix, name, extension);
            if (!_imageCache.ContainsKey(resourceName))
            {
                _imageCache.Add(resourceName, Image.FromStream(callingAssembly.GetManifestResourceStream(resourceName)));
            }
            return _imageCache[resourceName];            
        }
    }
}
