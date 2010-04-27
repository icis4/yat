//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace MKY.Test
{
	/// <summary>
	/// This static class provides the actual settings to parametrize test cases.
	/// </summary>
	public static class SettingsDefaults
	{
		static Settings staticSettings;

		static SettingsDefaults()
		{
			staticSettings = new Settings();
		}

		/// <summary>
		/// Creates the configuration.
		/// </summary>
		/// <param name="name">The name.</param>
		public static void CreateConfiguration(string name)
		{
			staticSettings.SettingsCollection.CreateConfiguration(name);
		}

		/// <summary>
		/// Adds the test setting of the given key.
		/// </summary>
		public static void AddSetting(string key, string value)
		{
			staticSettings.SettingsCollection.Add(key, value);
		}

		/// <summary>
		/// Adds the test setting of the given key.
		/// </summary>
		public static void AddSetting(string key, bool value)
		{
			staticSettings.SettingsCollection.Add(key, value.ToString());
		}

		/// <summary>
		/// Adds the test setting of the given key.
		/// </summary>
		public static void AddSetting(string key, char value)
		{
			staticSettings.SettingsCollection.Add(key, value.ToString());
		}

		/// <summary>
		/// Adds the test setting of the given key.
		/// </summary>
		public static void AddSetting(string key, byte value)
		{
			staticSettings.SettingsCollection.Add(key, value.ToString());
		}

		/// <summary>
		/// Adds the test setting of the given key.
		/// </summary>
		public static void AddSetting(string key, int value)
		{
			staticSettings.SettingsCollection.Add(key, value.ToString());
		}

		/// <summary>
		/// Adds the test setting of the given key.
		/// </summary>
		public static void AddSetting(string key, long value)
		{
			staticSettings.SettingsCollection.Add(key, value.ToString());
		}

		/// <summary>
		/// Saves the settings dictionary under the default file path.
		/// </summary>
		public static void SaveToSolutionFile()
		{
			staticSettings.SaveToSolutionFile();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
