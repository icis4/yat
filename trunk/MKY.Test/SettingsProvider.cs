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
	public static class SettingsProvider
	{
		static Settings staticSettings;

		static SettingsProvider()
		{
			staticSettings = new Settings();
			staticSettings.LoadFromExistingFiles();
		}

		/// <summary>
		/// Gets the test setting of the given key.
		/// </summary>
		public static bool TryGetSetting(string key, ref string value)
		{
			string valueString;
			if (staticSettings.SettingsCollection.TryGetValue(key, out valueString))
			{
				value = valueString;
				return (true);
			}
			return (false);
		}

		/// <summary>
		/// Gets the test setting of the given key.
		/// </summary>
		public static bool TryGetSetting(string key, ref bool value)
		{
			string valueString;
			if (staticSettings.SettingsCollection.TryGetValue(key, out valueString))
			{
				if (bool.TryParse(valueString, out value))
					return (true);
			}
			return (false);
		}

		/// <summary>
		/// Gets the test setting of the given key.
		/// </summary>
		public static bool TryGetSetting(string key, ref char value)
		{
			string valueString;
			if (staticSettings.SettingsCollection.TryGetValue(key, out valueString))
			{
				if (char.TryParse(valueString, out value))
					return (true);
			}
			return (false);
		}

		/// <summary>
		/// Gets the test setting of the given key.
		/// </summary>
		public static bool TryGetSetting(string key, ref byte value)
		{
			string valueString;
			if (staticSettings.SettingsCollection.TryGetValue(key, out valueString))
			{
				if (byte.TryParse(valueString, out value))
					return (true);
			}
			return (false);
		}

		/// <summary>
		/// Gets the test setting of the given key.
		/// </summary>
		public static bool TryGetSetting(string key, ref int value)
		{
			string valueString;
			if (staticSettings.SettingsCollection.TryGetValue(key, out valueString))
			{
				if (int.TryParse(valueString, out value))
					return (true);
			}
			return (false);
		}

		/// <summary>
		/// Gets the test setting of the given key.
		/// </summary>
		public static bool TryGetSetting(string key, ref long value)
		{
			string valueString;
			if (staticSettings.SettingsCollection.TryGetValue(key, out valueString))
			{
				if (long.TryParse(valueString, out value))
					return (true);
			}
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
