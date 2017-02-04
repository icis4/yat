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
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable verbose output:
////#define DEBUG_VERBOSE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace MKY.IO.Ports
{
	/// <summary>
	/// List containing serial port IDs.
	/// </summary>
	[Serializable]
	public class SerialPortCollection : List<SerialPortId>
	{
		private static string[]                   staticPortNamesCache; // = null;
		private static object                     staticPortNamesCacheSyncObj = new object();

		private static Dictionary<string, string> staticCaptionsCache; // = null;
		private static object                     staticCaptionsCacheSyncObj = new object();

		/// <summary></summary>
		public SerialPortCollection()
			: base(SerialPortId.LastStandardPortNumber - SerialPortId.FirstStandardPortNumber + 1)
		{
		}

		/// <summary></summary>
		public SerialPortCollection(IEnumerable<SerialPortId> rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Fills list with all ports from <see cref="SerialPortId.FirstStandardPortNumber"/> to
		/// <see cref="SerialPortId.LastStandardPortNumber"/>.
		/// </summary>
		public virtual void FillWithStandardPorts()
		{
			lock (this)
			{
				Clear();

				for (int i = SerialPortId.FirstStandardPortNumber; i <= SerialPortId.LastStandardPortNumber; i++)
					Add(new SerialPortId(i));

				Sort();
			}
		}

		/// <summary>
		/// Fills list with all ports from <see cref="System.IO.Ports.SerialPort.GetPortNames()"/>.
		/// </summary>
		/// <param name="retrieveCaptions">
		/// On request, this method queries the port captions from the system.
		/// Attention, this may take quite some time, depending on the available ports.
		/// Therefore, the default value is <c>false</c>.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public virtual void FillWithAvailablePorts(bool retrieveCaptions = false)
		{
			lock (this)
			{
				Clear();

				DebugVerboseIndent("Retrieving ports of local machine...");
				foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
				{
					DebugVerboseIndent(portName);
					Add(new SerialPortId(portName));
					DebugVerboseUnindent();
				}
				DebugVerboseUnindent("...done");

				Sort();
			}

			if (retrieveCaptions)
				RetrieveCaptions(true);
		}

		/// <summary>
		/// Returns whether the given port name is available according to <see cref="System.IO.Ports.SerialPort.GetPortNames()"/>.
		/// </summary>
		public static bool IsAvailable(string portName)
		{
			if (!string.IsNullOrEmpty(portName))
			{
				SerialPortCollection c = new SerialPortCollection();
				c.FillWithAvailablePorts(false);

				return (c.Contains(portName));
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Returns whether the given port name is contained in the static cache.
		/// </summary>
		public static bool CacheContains(string portName)
		{
			if ((staticPortNamesCache != null) && (!string.IsNullOrEmpty(portName)))
			{
				return (Array.IndexOf(staticPortNamesCache, portName) >= 0);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve to caption
		/// that is associated with the serial port.
		/// </summary>
		/// <remarks>
		/// Attention, this may take quite some time, depending on the available ports.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public virtual void RetrieveCaptions(bool forceRetrieveFromSystem = false)
		{
			bool useCaptionsFromCache;

			lock (staticPortNamesCacheSyncObj)
			{
				if (staticPortNamesCache == null)
				{
					staticPortNamesCache = System.IO.Ports.SerialPort.GetPortNames();
					useCaptionsFromCache = false;
				}
				else
				{
					string[] formerCache = (string[])staticPortNamesCache.Clone();
					staticPortNamesCache = System.IO.Ports.SerialPort.GetPortNames();
					useCaptionsFromCache = ArrayEx.ElementsEqual(staticPortNamesCache, formerCache);
				}
			}

			lock (staticCaptionsCacheSyncObj)
			{
				if ((staticCaptionsCache == null) || (!useCaptionsFromCache))
				{
					staticCaptionsCache = SerialPortSearcher.GetCaptionsFromSystem();

					DebugVerbose("Captions retrieved from system");
				}
				else
				{
					DebugVerbose("Captions cache is up-to-date");
				}

				lock (this)
				{
					foreach (SerialPortId portId in this)
					{
						string portName = portId.Name;
						if (staticCaptionsCache.ContainsKey(portName))
							portId.SetCaptionFromSystem(staticCaptionsCache[portName]);
					}
				}
			}
		}

		/// <summary>
		/// Checks all ports whether they are currently in use and marks them.
		/// </summary>
		/// <remarks>
		/// In .NET, no class provides a method to retrieve whether a port is currently in use or
		/// not. Therefore, this method actively tries to open every port! This may take quite some
		/// time, depending on the available ports.
		/// <remarks>
		/// </remarks>
		/// </remarks>
		/// <param name="portChangedCallback">
		/// Callback delegate, can be used to get an event each time a new port is being tried to
		/// be opened. Set the <see cref="SerialPortChangedAndCancelEventArgs.Cancel"/> property
		/// to <c>true</c> to cancel port scanning.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public virtual void DetectPortsInUse(EventHandler<SerialPortChangedAndCancelEventArgs> portChangedCallback = null)
		{
			lock (this)
			{
				foreach (SerialPortId portId in this)
				{
					if (portChangedCallback != null)
					{
						SerialPortChangedAndCancelEventArgs e = new SerialPortChangedAndCancelEventArgs(portId);

						EventHelper.FireSync<SerialPortChangedAndCancelEventArgs>(portChangedCallback, this, e);

						if (e.Cancel)
							break;
					}

					using (SerialPortEx p = new SerialPortEx(portId)) // Use SerialPortEx to prevent potential 'ObjectDisposedException'.
					{
						try
						{
							p.Open();
							p.Close();
							portId.IsInUse = false;
						}
						catch
						{
							portId.IsInUse = true;
						}
					}
				}
			}
		}

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerbose(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);
		}

		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseIndent(string message = null)
		{
			DebugVerbose(message);
			Debug.Indent();
		}

		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseUnindent(string message = null)
		{
			Debug.Unindent();
			DebugVerbose(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
