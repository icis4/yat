//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management;

using MKY.Diagnostics;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Provides methods to search for serial ports.
	/// </summary>
	public static class SerialPortSearcher
	{
		/// <summary>
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve the captions that is associated with the serial ports.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		/// <remarks>
		/// WMI calls the captions 'descriptions'. But from a user's point of view these are rather captions.
		/// </remarks>
		/// <remarks>
		/// If there is a need to manually browse through the WMI entries, use a tool like the "WMI Explorer".
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Emphasizes that this is a call to underlying system functions.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public static Dictionary<string, string> GetCaptionsFromSystem()
		{
			var result = new Dictionary<string, string>();

			try
			{
				var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
				foreach (var obj in searcher.Get())
				{
					try
					{
						// Check all objects with given GUID (results in all LPT and COM ports except modems):
						if ((obj["ClassGuid"] != null) && (StringEx.EqualsOrdinalIgnoreCase(obj["ClassGuid"].ToString(), "{4D36E978-E325-11CE-BFC1-08002BE10318}")))
						{
							if ((obj["Caption"] != null) && (obj["Description"] != null))
							{
								// "Caption" contains something like "Serial On USB Port (COM2)":
								var m = SerialPortId.StandardPortNameWithParenthesesRegex.Match(obj["Caption"].ToString());
								if (m.Success)
								{
									int portNumber;  // m.Value is e.g. "(COM2) thus [1] is "2".
									if (int.TryParse(m.Groups[1].Value, out portNumber))
									{
										// Retrieve description:
										string portName = SerialPortId.StandardPortNumberToString(portNumber);
										if (!result.ContainsKey(portName))
											result.Add(portName, obj["Description"].ToString());
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(typeof(SerialPortSearcher), ex, "Exception while accessing WMI!");
					}
				}

				searcher = new ManagementObjectSearcher("SELECT * FROM Win32_POTSModem");
				foreach (var obj in searcher.Get())
				{
					try
					{
						if ((obj["AttachedTo"] != null) && (obj["Description"] != null))
						{
							// "AttachedTo" contains something like "COM2":
							var m = SerialPortId.StandardPortNameOnlyRegex.Match(obj["AttachedTo"].ToString());
							if (m.Success)
							{
								int portNumber;  // m.Value is e.g. "COM2 thus [1] is "2".
								if (int.TryParse(m.Groups[1].Value, out portNumber))
								{
									// Retrieve description:
									string portName = SerialPortId.StandardPortNumberToString(portNumber);
									if (!result.ContainsKey(portName))
										result.Add(portName, obj["Description"].ToString());
								}
							}
						}
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(typeof(SerialPortSearcher), ex, "Exception while accessing WMI!");
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(SerialPortSearcher), ex, "Exception while accessing WMI!");
			}

			return (result);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
