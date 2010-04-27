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

using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

using MKY.Utilities.Diagnostics;

namespace MKY.Test
{
	/// <summary>
	/// This class provides settings to parametrize test cases. Test parameters are stored in an
	/// XML file. Default parameters must be stored in a file in the solution structure. Specific
	/// or all parameters can be overriden by local settings under a given environment variable
	/// path.
	/// </summary>
	/// <example>
	/// Example test parameters
	/// -----------------------
	/// <code>
	/// {
	///     MKY.Test.SettingsDefaults.AddSetting("SerialPortAIsAvailable", false);
	///     MKY.Test.SettingsDefaults.AddSetting("SerialPortBIsAvailable", false);
	///     
	///     MKY.Test.SettingsDefaults.AddSetting("SerialPortA", "COM1");
	///     MKY.Test.SettingsDefaults.AddSetting("SerialPortB", "COM2");
	///     
	///     MKY.Test.SettingsDefaults.AddSetting("SerialPortsAreInterconnected", false);
	/// }
	/// </code>
	/// The corresponding settings file example is "ExampleSettings.MKY.Test".
	/// </example>
	public class Settings
	{
		private const string SettingsSection = "MKY.Test";
		private const string SettingsFileName = "Settings.MKY.Test";
		private const string UserSettingsPathVariableName = "MKY_TEST_PATH";

		private SettingsCollection settingsCollection = new SettingsCollection();

		/// <summary>
		/// Gets the test parameter settings.
		/// </summary>
		public SettingsCollection SettingsCollection
		{
			get { return (this.settingsCollection); }
		}

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Loads the solution settings and overrides/adds settings by local settings if given.
		/// </summary>
		public void LoadFromExistingFiles()
		{
			// Reset settings dictionary.
			this.settingsCollection.Clear();

			// Copy existing project settings.
			SettingsCollection projectSettings = LoadSolutionSettings();
			if (projectSettings != null)
			{
				foreach (string key in projectSettings.Keys)
				{
					this.settingsCollection.Add(key, projectSettings[key]);
				}
			}

			// Override/add user settings where available.
			SettingsCollection userSettings = LoadUserSettings();
			if (userSettings != null)
			{
				foreach (string key in userSettings.Keys)
				{
					if (this.settingsCollection.ContainsKey(key))
						this.settingsCollection[key] = userSettings[key];
					else
						this.settingsCollection.Add(key, userSettings[key]);
				}
			}
		}

		/// <summary>
		/// Saves the settings under the solution file path.
		/// </summary>
		public void SaveToSolutionFile()
		{
			SaveSolutionSettings(this.settingsCollection);
		}

		#endregion

		#region Private Methods > Load
		//==========================================================================================
		// Private Methods > Load
		//==========================================================================================

		/// <summary>
		/// Loads the solution's test settings from the nearmost directory.
		/// </summary>
		private SettingsCollection LoadSolutionSettings()
		{
			AppDomain.CurrentDomain.SetupInformation.ConfigurationFile

			ConfigurationManager. GetSection(SettingsSection);
			string solutionDirectory = Path.GetDirectoryName(dte.Solution.FullName);

			DirectoryInfo di = new DirectoryInfo(solutionDirectory);
			//DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
			while (di != null)
			{
				string filePath = di.FullName + Path.DirectorySeparatorChar + SettingsFileName;
				if (File.Exists(filePath))
				{
					SettingsCollection settings = LoadSettingsFromFile(filePath);
					if (settings != null)
						return (settings);
				}
				di = di.Parent;
			}
			return (null);
		}

		/// <summary>
		/// Loads the user's test settings from the path given by the environment variable.
		/// </summary>
		private SettingsCollection LoadUserSettings()
		{
			string userSettingsPath = Environment.GetEnvironmentVariable(UserSettingsPathVariableName);
			if (userSettingsPath != "")
			{
				string filePath = userSettingsPath + Path.DirectorySeparatorChar + SettingsFileName;
				if (File.Exists(filePath))
				{
					SettingsCollection settings = LoadSettingsFromFile(filePath);
					if (settings != null)
						return (settings);
				}
			}
			return (null);
		}

		private SettingsCollection LoadSettingsFromFile(string filePath)
		{
			try
			{
				SettingsCollection settings;
				using (StreamReader sr = new StreamReader(filePath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(SettingsCollection));
					settings = (SettingsCollection)serializer.Deserialize(sr);
				}
				return (settings);
			}
			catch (Exception ex)
			{
				XDebug.WriteException(this, ex);
			}

			// If nothing found, return <c>null</c>.
			return (null);
		}

		#endregion

		#region Private Methods > Save
		//==========================================================================================
		// Private Methods > Save
		//==========================================================================================

		/// <summary>
		/// Saves the settings in the solution directory.
		/// </summary>
		/// <remarks>
		/// Cannot make use of EnvDTE. EnvDTE doesn't work when Visual Studio is debugging NUnit.
		/// Therefore, assume that the directory structure is 'Solution'\'Project'\bin\'Debug|Release'.
		/// </remarks>
		private void SaveSolutionSettings(SettingsCollection settings)
		{
			DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
			for (int i = 3; i > 0; i--)
				di = di.Parent;

			string filePath = di.FullName + Path.DirectorySeparatorChar + SettingsFileName;
			SaveSettingsToFile(filePath, settings);
		}

		private void SaveSettingsToFile(string filePath, SettingsCollection settings)
		{
			try
			{
				using (StreamWriter sw = new StreamWriter(filePath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(SettingsCollection));
					serializer.Serialize(sw, settings);
				}
			}
			catch (Exception ex)
			{
				XDebug.WriteException(this, ex);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
