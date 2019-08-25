﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.15
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
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

namespace MKY.IO.Usb
{
	/// <summary>
	/// List containing USB device information.
	/// </summary>
	[Serializable]
	public class DeviceCollection : List<DeviceInfo>
	{
		private DeviceClass deviceClass = DeviceClass.Any;
		private Guid        classGuid   = Guid.Empty;

		/// <summary></summary>
		public DeviceCollection()
		{
		}

		/// <summary></summary>
		public DeviceCollection(DeviceClass deviceClass)
		{
			this.deviceClass = deviceClass;
			this.classGuid = Device.GetGuidFromDeviceClass(deviceClass);
		}

		/// <summary></summary>
		public DeviceCollection(IEnumerable<DeviceInfo> rhs)
			: base(rhs)
		{
			var casted = (rhs as DeviceCollection);
			if (casted != null)
			{
				this.deviceClass = casted.deviceClass;
				this.classGuid   = casted.classGuid;
			}
		}

		/// <summary>
		/// Fills list with the available USB devices.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual void FillWithAvailableDevices(bool retrieveStringsFromDevice = true)
		{
			lock (this)
			{
				Clear();

				DebugVerboseIndent("Retrieving connected USB devices...");
				foreach (DeviceInfo di in Device.GetDevicesFromGuid(this.classGuid, retrieveStringsFromDevice))
				{
					DebugVerboseIndent(di);
					Add(di);
					DebugVerboseUnindent();
				}
				DebugVerboseUnindent("...done");

				Sort();
			}
		}

		/// <summary>
		/// Determines whether an element is in the collection.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the collection. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// <c>true</c> if item is found in the collection; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public virtual bool ContainsVidPid(DeviceInfo item)
		{
			lock (this)
			{
				foreach (DeviceInfo di in this)
				{
					if (di.EqualsVidPid(item))
						return (true);
				}

				return (false);
			}
		}

		/// <summary>
		/// Searches for an element that matches the <paramref name="item"/>, and returns the
		/// first occurrence within the entire collection.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the collection. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// The first element that matches the <paramref name="item"/>, if found; otherwise, –1.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public virtual DeviceInfo FindVidPid(DeviceInfo item)
		{
			lock (this)
			{
				EqualsVidAndPid predicate = new EqualsVidAndPid(item);
				return (Find(predicate.Match));
			}
		}

		/// <summary>
		/// Searches for an element that matches the <paramref name="item"/>, and returns the
		/// zero-based index of the first occurrence within the collection.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the collection. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of an element that matches the
		/// <paramref name="item"/>, if found; otherwise, –1.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public virtual int FindIndexVidPid(DeviceInfo item)
		{
			lock (this)
			{
				EqualsVidAndPid predicate = new EqualsVidAndPid(item);
				return (FindIndex(predicate.Match));
			}
		}

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseIndent(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);

			Debug.Indent();
		}

		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseUnindent(string message = null)
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