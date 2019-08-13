//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2009 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Management;

using MKY.Utilities.Diagnostics;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Provides methods to search for serial ports.
	/// </summary>
	public static class SerialPortSearcher
	{
		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve to description
		///   that is associated with the serial ports.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public static Dictionary<string, string> GetDescriptionsFromSystem()
		{
			Dictionary<string, string> descriptions = new Dictionary<string, string>();

			try
			{
				// Use a tool like "WMI Explorer" to browse through the WMI entries

				ManagementObjectSearcher searcher;

				searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
				foreach (ManagementObject obj in searcher.Get())
				{
					// Check all objects with given GUID
					// Results in all LPT and COM ports except modems
					if (obj["ClassGuid"].ToString() == "{4D36E978-E325-11CE-BFC1-08002BE10318}")
					{
						// "Caption" contains something like "Serial On USB Port (COM2)"
						Match m = SerialPortId.StandardPortNameWithParenthesesRegex.Match(obj["Caption"].ToString());
						if (m.Success)
						{
							int portNumber;
							if (int.TryParse(m.Groups[1].Value, out portNumber))
							{
								string portName = SerialPortId.StandardPortNumberToString(portNumber);
								if (!descriptions.ContainsKey(portName))
									descriptions.Add(portName, obj["Description"].ToString());
							}
						}
					}
				}

				searcher = new ManagementObjectSearcher("SELECT * FROM Win32_POTSModem");
				foreach (ManagementObject obj in searcher.Get())
				{
					// "AttachedTo" contains something like "COM1"
					Match m = SerialPortId.StandardPortNameOnlyRegex.Match(obj["AttachedTo"].ToString());
					if (m.Success)
					{
						int portNumber;
						if (int.TryParse(m.Groups[1].Value, out portNumber))
						{
							string portName = SerialPortId.StandardPortNumberToString(portNumber);
							if (!descriptions.ContainsKey(portName))
								descriptions.Add(portName, obj["Description"].ToString());
						}
					}
				}
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(SerialPortSearcher), ex);
			}

			return (descriptions);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================