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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MKY.Test
{
	#region StringKeyValuePair
	//==========================================================================================
	// StringKeyValuePair
	//==========================================================================================

	/// <summary>
	/// Serializable string key/value-pair.
	/// </summary>
	/// <remarks>
	/// Required because <see cref="T:KeyValuePair`1"/> doesn't properly serialize
	/// its contents even though it is marked <see cref="SerializableAttribute"/>.
	/// </remarks>
	[Serializable]
	public struct StringKeyValuePair
	{
		private string key;
		private string value;

		/// <summary>
		/// Initializes a new instance of the <see cref="StringKeyValuePair"/> struct.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public StringKeyValuePair(string key, string value)
		{
			this.key = key;
			this.value = value;
		}

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		[XmlElement("Key")]
		public string Key
		{
			get { return (this.key); }
			set { this.key = value;  }
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[XmlElement("Value")]
		public string Value
		{
			get { return (this.value); }
			set { this.value = value;  }
		}
	}

	#endregion

	#region NamedStringDictionary
	//==========================================================================================
	// NamedStringDictionary
	//==========================================================================================

	/// <summary>
	/// A collection containing one or more sets of test settings.
	/// </summary>
	[Serializable]
	public class NamedStringDictionary
	{
		private string name;
		private Dictionary<string, string> dictionary;

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedStringDictionary"/> class.
		/// </summary>
		public NamedStringDictionary()
			: this("")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedStringDictionary"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public NamedStringDictionary(string name)
		{
			this.name = name;
			this.dictionary = new Dictionary<string, string>();
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("Name")]
		public string Name
		{
			get { return (this.name); }
			set { this.name = value;  }
		}

		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>The settings.</value>
		[XmlElement("Settings")]
		public List<StringKeyValuePair> Settings
		{
			get
			{
				List<StringKeyValuePair> l = new List<StringKeyValuePair>(dictionary.Count);
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, string> kvp in dictionary)
						l.Add(new StringKeyValuePair(kvp.Key, kvp.Value));
				}
				return (l);
			}
			set
			{
				dictionary.Clear();

				if (value != null)
				{
					foreach (StringKeyValuePair kvp in value)
						dictionary.Add(kvp.Key, kvp.Value);
				}
			}
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		[XmlIgnore]
		public Dictionary<string, string>.KeyCollection Keys
		{
			get { return (this.dictionary.Keys); }
		}

		/// <summary>
		/// Gets the <see cref="System.String"/> with the specified key.
		/// </summary>
		[XmlIgnore]
		public string this[string key]
		{
			get { return (this.dictionary[key]); }
			set { this.dictionary[key] = value;  }
		}

		/// <summary>
		/// Clears all settings.
		/// </summary>
		public void Clear()
		{
			dictionary.Clear();
		}

		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Add(string key, string value)
		{
			this.dictionary.Add(key, value);
		}

		/// <summary>
		/// Determines whether the specified key contains key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsKey(string key)
		{
			return (this.dictionary.ContainsKey(key));
		}

		/// <summary>
		/// Tries the get value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		/// <c>true</c> if the value at the specified key could be retrieved; otherwise, <c>false</c>.
		/// </returns>
		public bool TryGetValue(string key, out string value)
		{
			return (this.dictionary.TryGetValue(key, out value));
		}
	}

	#endregion

	#region SettingsCollection
	//==========================================================================================
	// SettingsCollection
	//==========================================================================================

	/// <summary>
	/// A collection containing one or more sets of test settings.
	/// </summary>
	[Serializable]
	public class SettingsCollection
	{
		private string selectedConfigurationName;
		private List<NamedStringDictionary> configurations;

		/// <summary>
		/// Initializes a new instance of the <see cref="SettingsCollection"/> class.
		/// </summary>
		public SettingsCollection()
			: this("")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SettingsCollection"/> class.
		/// </summary>
		/// <param name="selectedConfigurationName">Name of the selected configuration.</param>
		public SettingsCollection(string selectedConfigurationName)
		{
			this.selectedConfigurationName = selectedConfigurationName;
			this.configurations = new List<NamedStringDictionary>();
		}

		/// <summary>
		/// Gets or sets the name of the selected configuration.
		/// </summary>
		/// <value>The name of the selected configuration.</value>
		[XmlElement("SelectedConfigurationName")]
		public string SelectedConfigurationName
		{
			get { return (this.selectedConfigurationName); }
			set { this.selectedConfigurationName = value;  }
		}

		/// <summary>
		/// Gets or sets the configurations.
		/// </summary>
		/// <value>The configurations.</value>
		[XmlElement("Configurations")]
		public List<NamedStringDictionary> Configurations
		{
			get { return (this.configurations); }
			set { this.configurations = value;  }
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		[XmlIgnore]
		public Dictionary<string, string>.KeyCollection Keys
		{
			get
			{
				int i = SelectedConfigurationIndex;
				if (i >= 0)
					return (this.configurations[i].Keys);

				return (null);
			}
		}

		/// <summary>
		/// Gets the <see cref="System.String"/> with the specified key.
		/// </summary>
		[XmlIgnore]
		public string this[string key]
		{
			get
			{
				int i = SelectedConfigurationIndex;
				if (i >= 0)
					return (this.configurations[i][key]);

				return (null);
			}
			set
			{
				int i = SelectedConfigurationIndex;
				if (i >= 0)
					this.configurations[i][key] = value;
			}
		}

		/// <summary>
		/// Creates the configuration.
		/// </summary>
		/// <param name="name">The name.</param>
		public void CreateConfiguration(string name)
		{
			this.configurations.Add(new NamedStringDictionary(name));
			this.selectedConfigurationName = name;
		}

		/// <summary>
		/// Clears all settings.
		/// </summary>
		public void Clear()
		{
			configurations.Clear();
		}

		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Add(string key, string value)
		{
			int i = SelectedConfigurationIndex;
			if (i >= 0)
				this.configurations[i].Add(key, value);
		}

		/// <summary>
		/// Determines whether the specified key contains key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsKey(string key)
		{
			int i = SelectedConfigurationIndex;
			if (i >= 0)
				return (this.configurations[i].ContainsKey(key));

			return (false);
		}

		/// <summary>
		/// Tries the get value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		/// <c>true</c> if the value at the specified key could be retrieved; otherwise, <c>false</c>.
		/// </returns>
		public bool TryGetValue(string key, out string value)
		{
			int i = SelectedConfigurationIndex;
			if (i >= 0)
				return (this.configurations[i].TryGetValue(key, out value));

			value = null;
			return (false);
		}

		private int SelectedConfigurationIndex
		{
			get
			{
				for (int i = 0; i < Configurations.Count; i++)
				{
					if (Configurations[i].Name == this.selectedConfigurationName)
						return (i);
				}
				return (-1);
			}
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
