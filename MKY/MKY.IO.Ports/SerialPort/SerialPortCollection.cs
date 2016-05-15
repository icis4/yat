//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
		static string[]                   staticPortNamesCache; // = null
		static object                     staticPortNamesCacheSyncObj = new object();

		static Dictionary<string, string> staticCaptionsCache; // = null
		static object                     staticCaptionsCacheSyncObj = new object();

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
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve to caption
		/// that is associated with the serial port.
		/// </summary>
		/// <remarks>
		/// Attention, this may take quite some time, depending on the available ports.
		/// </remarks>
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
					useCaptionsFromCache = ArrayEx.ValuesEqual(staticPortNamesCache, formerCache);
				}
			}

			lock (staticCaptionsCacheSyncObj)
			{
				if ((staticCaptionsCache == null) || (!useCaptionsFromCache))
				{
					staticCaptionsCache = SerialPortSearcher.GetCaptionsFromSystem();

					Debug.WriteLine(GetType() + ": Captions retrieved from system");
				}
				else
				{
					Debug.WriteLine(GetType() + ": Captions cache is up-to-date");
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

					using (System.IO.Ports.SerialPort p = new System.IO.Ports.SerialPort(portId))
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
		private void DebugVerboseIndent(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);

			Debug.Indent();
		}

		[Conditional("DEBUG_VERBOSE")]
		private void DebugVerboseUnindent(string message = null)
		{
			Debug.Unindent();

			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
