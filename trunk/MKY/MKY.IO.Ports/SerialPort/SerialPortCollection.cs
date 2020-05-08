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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Text;
using System.Threading;
using System.Xml.Serialization;

using MKY.Diagnostics;

#endregion

namespace MKY.IO.Ports
{
	/// <summary>
	/// List containing serial port IDs.
	/// </summary>
	[Serializable]
	public class SerialPortCollection : List<SerialPortId>
	{
		/// <summary>
		/// Occurs when the collection requests the 'InUseText' for the given port.
		/// </summary>
		public static event EventHandler<SerialPortInUseLookupEventArgs> InUseLookupRequest;

		private static string[]                   staticPortNamesCache; // = null;
		private static object                     staticPortNamesCacheSyncObj = new object();

		private static Dictionary<string, string> staticCaptionsCache; // = null;
		private static object                     staticCaptionsCacheSyncObj = new object();

		/// <summary>
		/// A dedicated event helper to allow ignoring the 'ThreadAbortException' when cancelling.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(SerialPortCollection).FullName);

		/// <summary>
		/// The port that is currently in use, e.g. "(in use by this serial port)" of "COM1 - (in use by this serial port)".
		/// </summary>
		[XmlIgnore]
		public InUseInfo ActivePortInUseInfo { get; set; } // = null;

		/// <summary>
		/// The text which is shown when port is currently in use, e.g. "(in use by other app)" of "COM1 - (in use by other app)".
		/// </summary>
		[XmlIgnore]
		public string OtherAppInUseText { get; set; } = InUseInfo.OtherAppInUseTextDefault;

		/// <summary></summary>
		public SerialPortCollection()
			: base(SerialPortId.TypicalStandardPortCount)
		{
		}

		/// <summary></summary>
		public SerialPortCollection(IEnumerable<SerialPortId> rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Fills list with all ports from <see cref="SerialPortId.FirstTypicalStandardPortNumber"/> to
		/// <see cref="SerialPortId.LastTypicalStandardPortNumber"/>.
		/// </summary>
		public virtual void FillWithTypicalStandardPorts()
		{
			lock (this)
			{
				Clear();

				for (int i = SerialPortId.FirstTypicalStandardPortNumber; i <= SerialPortId.LastTypicalStandardPortNumber; i++)
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
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

					DebugVerbose("Captions retrieved from system");
				}
				else
				{
					DebugVerbose("Captions cache is up-to-date");
				}

				lock (this)
				{
					foreach (var portId in this)
					{
						var portName = portId.Name;
						if (staticCaptionsCache.ContainsKey(portName))
							portId.SetCaptionFromSystem(staticCaptionsCache[portName]);
					}
				}
			}
		}

		/// <summary></summary>
		protected virtual List<InUseInfo> RetrieveOtherPortInUseLookup()
		{
			List<InUseInfo> otherPortInUseLookup = null;

			if (InUseLookupRequest != null)
			{
				otherPortInUseLookup = OnInUseLookupRequest();

				if (ActivePortInUseInfo != null)
					otherPortInUseLookup.RemoveAll(inUse => (inUse.UseId == ActivePortInUseInfo.UseId));
			}

			return (otherPortInUseLookup);
		}

		/// <summary>
		/// Checks all ports whether they are currently in use and marks them.
		/// </summary>
		/// <remarks>
		/// In .NET, no class provides a method to retrieve whether a port is currently in use or
		/// not. Therefore, this method actively tries to open every port! This may take quite some
		/// time, depending on the available ports.
		/// </remarks>
		/// <param name="portChangedCallback">
		/// Callback delegate, can be used to get an event each time a new port is being tried to
		/// be opened. Set the <see cref="SerialPortChangedAndCancelEventArgs.Cancel"/> property
		/// to <c>true</c> to cancel port scanning.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void DetectPortsThatAreInUse(EventHandler<SerialPortChangedAndCancelEventArgs> portChangedCallback = null)
		{
			var otherPortInUseLookup = RetrieveOtherPortInUseLookup();

			lock (this)
			{
				foreach (var portId in this)
				{
					// Handle callback for current port:
					if (portChangedCallback != null)
					{
						var e = new SerialPortChangedAndCancelEventArgs(portId);
						this.eventHelper.RaiseSync<SerialPortChangedAndCancelEventArgs>(portChangedCallback, this, e);
						if (e.Cancel)
							break;
					}

					// Lookup and evaluate current port:
					DetectWhetherPortIsInUse(portId, otherPortInUseLookup);
				}
			}
		}

		/// <summary>
		/// Checks the port whether it is currently in use and marks it.
		/// </summary>
		/// <remarks>
		/// In .NET, no class provides a method to retrieve whether a port is currently in use or
		/// not. Therefore, this method actively tries to open every port! This may take quite some
		/// time, depending on the available ports.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void DetectWhetherPortIsInUse(SerialPortId portId)
		{
			var otherPortInUseLookup = RetrieveOtherPortInUseLookup();
			DetectWhetherPortIsInUse(portId, otherPortInUseLookup);
		}

		/// <summary>
		/// Detects whether the port is in use.
		/// </summary>
		protected virtual void DetectWhetherPortIsInUse(SerialPortId portId, List<InUseInfo> otherPortInUseLookup)
		{
			// Lookup:
			bool isInUseByActivePort = false;
			bool isOpenByActivePort = false;
			if ((ActivePortInUseInfo != null) && (ActivePortInUseInfo.PortName == portId.Name))
			{
				isInUseByActivePort = true;
				isOpenByActivePort = ActivePortInUseInfo.IsOpen;
			}

			List<InUseInfo> otherPortInUseInfo = null;
			bool isInUseByOtherPort = false;
			bool isOpenByOtherPort = false;
			if (otherPortInUseLookup != null)
			{
				otherPortInUseInfo = otherPortInUseLookup.FindAll(inUse => (inUse.PortName == portId.Name));
				if (otherPortInUseInfo != null)
				{
					foreach (var statement in otherPortInUseInfo)
					{
						isInUseByOtherPort = true;

						if (statement.IsOpen)
							isOpenByOtherPort = true;
					}
				}
			}

			// Evaluate:
			if (isOpenByActivePort || isOpenByOtherPort)
			{
				portId.IsInUse = true;
				portId.InUseText = ComposeInUseText(isInUseByActivePort, ActivePortInUseInfo, otherPortInUseInfo);
			}
			else // Not open according to statements, but could still be open, e.g. by external application:
			{
				DetectWhetherPortIsInUse(portId, isInUseByActivePort, ActivePortInUseInfo, isInUseByOtherPort, otherPortInUseInfo, OtherAppInUseText);
			}
		}

		/// <summary>
		/// Detects whether the port is in use and marks the port ID accordingly.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected static void DetectWhetherPortIsInUse(SerialPortId portId, bool isInUseByActivePort, InUseInfo activePortInUseInfo, bool isInUseByOtherPort, List<InUseInfo> otherPortInUseInfo, string otherAppInUseText = null)
		{
			using (var p = new SerialPortEx(portId)) // Use 'SerialPortEx' instead of 'SerialPort' to
			{                                        // prevent potential 'ObjectDisposedException'.
				try
				{
					p.Open();
					p.CloseNormally();

					// Could be opened, but could be selected by active or other port:
					if (isInUseByActivePort || isInUseByOtherPort)
					{
						portId.IsInUse = true;
						portId.InUseText = ComposeInUseText(isInUseByActivePort, activePortInUseInfo, otherPortInUseInfo);
					}
					else
					{
						portId.IsInUse = false;
					}
				}
				catch (ThreadAbortException ex)
				{
					DebugEx.WriteException(typeof(SerialPortCollection), ex, "DetectWhetherPortIsInUse() has detected a thread exception. It is ignored here but rethrown.");

					// Do not activate 'InUse', as a thread abort doesn't indicate that a port is in use.
					// An abort may happen when e.g. cancelling the 'SelectionWorker.DoWork()' method.
					// In such a case, re-throw so it can be handled by that method.

					throw; // Rethrow!
				}
				catch
				{
					portId.IsInUse = true;
					portId.InUseText = ComposeInUseText(isInUseByActivePort, activePortInUseInfo, otherPortInUseInfo, otherAppInUseText);
				}
			}
		}

		/// <summary>
		/// Composes the 'InUse' text.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected static string ComposeInUseText(bool isInUseByActivePort, InUseInfo activePortInUseInfo, List<InUseInfo> otherPortInUseInfo, string otherAppInUseText = null)
		{
			var inUseText = new StringBuilder();

			if (isInUseByActivePort && (activePortInUseInfo != null))
				inUseText.Append(activePortInUseInfo.InUseText); // "(in use by this serial port)"

			if ((otherPortInUseInfo != null) && (otherPortInUseInfo.Count > 0))
			{
				foreach (var statement in otherPortInUseInfo)
				{
					if (!string.IsNullOrEmpty(statement.InUseText))
					{
						if (inUseText.Length > 0)
							inUseText.Append(" ");

						inUseText.Append(statement.InUseText); // "(in use by ...)"
					}
				}
			}
			else if (!string.IsNullOrEmpty(otherAppInUseText))
			{
				inUseText.Append(otherAppInUseText); // "(in use by another application)"
			}

			return (inUseText.ToString());
		}

		/// <summary>
		/// Notifies the worker that a thread abort is about to happen soon.
		/// </summary>
		public virtual void NotifyThreadAbortWillHappen()
		{
			this.eventHelper.DiscardAllExceptions();
		}

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Event requires to retrieve a collection.")]
		protected virtual List<InUseInfo> OnInUseLookupRequest()
		{
			var e = new SerialPortInUseLookupEventArgs();
			this.eventHelper.RaiseSync<SerialPortInUseLookupEventArgs>(InUseLookupRequest, this, e);
			return (e.InUseLookup);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because <see cref="ConditionalAttribute"/> only works locally.
		/// </remarks>
		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerbose(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);
		}

		/// <remarks>
		/// <c>private</c> because <see cref="ConditionalAttribute"/> only works locally.
		/// </remarks>
		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseIndent(string message = null)
		{
			DebugVerbose(message);
			Debug.Indent();
		}

		/// <remarks>
		/// <c>private</c> because <see cref="ConditionalAttribute"/> only works locally.
		/// </remarks>
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
