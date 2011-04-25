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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;

using MKY.Diagnostics;

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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public static Dictionary<string, string> GetDescriptionsFromSystem()
		{
			Dictionary<string, string> descriptions = new Dictionary<string, string>();

			try
			{
				// If there is a need to manually browse through the WMI entries,
				//   use a tool like the "WMI Explorer".

				ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
				foreach (ManagementObject obj in searcher.Get())
				{
					try
					{
						// Check all objects with given GUID.
						// Results in all LPT and COM ports except modems.
						if ((obj["ClassGuid"] != null) && (StringEx.EqualsOrdinalIgnoreCase(obj["ClassGuid"].ToString(), "{4D36E978-E325-11CE-BFC1-08002BE10318}")))
						{
							if ((obj["Caption"] != null) && (obj["Description"] != null))
							{
								// "Caption" contains something like "Serial On USB Port (COM2)".
								Match m = SerialPortId.StandardPortNameWithParenthesesRegex.Match(obj["Caption"].ToString());
								if (m.Success)
								{
									int portNumber;
									if (int.TryParse(m.Groups[1].Value, out portNumber))
									{
										// Retrieve description.
										string portName = SerialPortId.StandardPortNumberToString(portNumber);
										if (!descriptions.ContainsKey(portName))
											descriptions.Add(portName, obj["Description"].ToString());
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(typeof(SerialPortSearcher), ex);
					}
				}

				searcher = new ManagementObjectSearcher("SELECT * FROM Win32_POTSModem");
				foreach (ManagementObject obj in searcher.Get())
				{
					try
					{
						if ((obj["AttachedTo"] != null) && (obj["Description"] != null))
						{
							// "AttachedTo" contains something like "COM1".
							Match m = SerialPortId.StandardPortNameOnlyRegex.Match(obj["AttachedTo"].ToString());
							if (m.Success)
							{
								int portNumber;
								if (int.TryParse(m.Groups[1].Value, out portNumber))
								{
									// Retrieve description.
									string portName = SerialPortId.StandardPortNumberToString(portNumber);
									if (!descriptions.ContainsKey(portName))
										descriptions.Add(portName, obj["Description"].ToString());
								}
							}
						}
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(typeof(SerialPortSearcher), ex);
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(SerialPortSearcher), ex);
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
