//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using MKY.Diagnostics;
using MKY.Xml;
using MKY.Xml.Serialization;

#endregion

namespace MKY.Settings
{
	/// <summary>
	/// Static utility class to provide basic settings handling methods.
	/// </summary>
	public static class SettingsHandler
	{
		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		#region Static Methods > Load
		//------------------------------------------------------------------------------------------
		// Static Methods > Load
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public static object LoadFromFile(string filePath, Type type, AlternateXmlElement[] alternateXmlElements, Type parentType)
		{
			// Try to open existing file of current version.
			if (File.Exists(filePath)) // First check for file to minimize exceptions thrown.
			{
				// Try to open existing file with default deserialization.
				// Always try this as this is the fastest way of deserialization.
				try
				{
					return (DeserializeFromFile(filePath, type));
				}
				catch { }

				if (alternateXmlElements == null)
				{
					// Try to open existing file with tolerant deserialization.
					try
					{
						return (TolerantDeserializeFromFile(filePath, type));
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(parentType, ex);
					}
				}
				else
				{
					// Try to open existing file with tolerant/alternate-tolerant deserialization.
					try
					{
						return (AlternateTolerantDeserializeFromFile(filePath, type, alternateXmlElements));
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(parentType, ex);
					}
				}
			}

			// If not successful, return <c>null</c>.
			return (null);
		}

		#region Static Methods > Load > Deserialize
		//------------------------------------------------------------------------------------------
		// Static Methods > Load > Deserialize
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static object DeserializeFromFile(string filePath, Type type)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				XmlSerializer serializer = new XmlSerializer(type);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}

		/// <summary></summary>
		public static object TolerantDeserializeFromFile(string filePath, Type type)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				TolerantXmlSerializer serializer = new TolerantXmlSerializer(type);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}

		/// <summary></summary>
		public static object AlternateTolerantDeserializeFromFile(string filePath, Type type, AlternateXmlElement[] alternateXmlElements)
		{
			object settings = null;
			using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
			{
				AlternateTolerantXmlSerializer serializer = new AlternateTolerantXmlSerializer(type, alternateXmlElements);
				settings = serializer.Deserialize(sr);
			}
			return (settings);
		}

		#endregion

		#endregion

		#region Static Methods > Save
		//------------------------------------------------------------------------------------------
		// Static Methods > Save
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public static void SaveToFile(string filePath, Type type, object settings, Type parentType)
		{
			string backup = filePath + IO.FileEx.BackupFileExtension;

			try
			{
				if (File.Exists(backup))
					File.Delete(backup);
				if (File.Exists(filePath))
					File.Move(filePath, backup);
			}
			catch { }

			try
			{
				SerializeToFile(filePath, type, settings);
			}
			catch
			{
				try
				{
					if (File.Exists(backup))
						File.Move(backup, filePath);
				}
				catch { }

				throw; // Re-throw!
			}
			finally
			{
				try
				{
					if (File.Exists(backup))
						File.Delete(backup);
				}
				catch { }
			}
		}

		#region Static Methods > Save > Serialize
		//------------------------------------------------------------------------------------------
		// Static Methods > Save > Serialize
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static void SerializeToFile(string filePath, Type type, object settings)
		{
			using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer(type);
				serializer.Serialize(sw, settings);
			}
		}

		#endregion

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
