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
// Copyright © 2010-2020 Matthias Kläy.
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
	/// List containing USB HID device information.
	/// </summary>
	[Serializable]
	public class HidDeviceCollection : DeviceCollection<HidDeviceInfo>
	{
		/// <summary></summary>
		public const int AnyUsagePage = -1;

		/// <summary></summary>
		public const int AnyUsageId = -1;

		private int usagePage = AnyUsagePage;
		private int usageId   = AnyUsageId;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public HidDeviceCollection(int usagePage = AnyUsagePage, int usageId = AnyUsageId)
			: base(DeviceClass.Hid)
		{
			this.usagePage = usagePage;
			this.usageId   = usageId;
		}

		/// <summary></summary>
		public HidDeviceCollection(IEnumerable<HidDeviceInfo> rhs)
			: base(rhs)
		{
			var casted = (rhs as HidDeviceCollection);
			if (casted != null)
			{
				this.usagePage = casted.usagePage;
				this.usageId   = casted.usageId;
			}
		}

		/// <summary>
		/// Fills list with the available USB HID devices.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Ser/HID' just happens to contain 'Ser'...")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override void FillWithAvailableDevices(bool retrieveStringsFromDevice = true)
		{
			lock (this)
			{
				Clear();

				DebugVerboseIndent("Retrieving connected USB HID devices...");
				foreach (var di in HidDevice.GetDevices(this.usagePage, this.usageId, retrieveStringsFromDevice))
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
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only
		/// <see cref="DeviceInfo.VendorId"/>, <see cref="DeviceInfo.ProductId"/> and
		/// <see cref="HidDeviceInfo.UsagePage"/>, <see cref="HidDeviceInfo.UsageId"/> are considered.
		/// </remarks>
		public virtual bool ContainsVidPidUsage(HidDeviceInfo item)
		{
			lock (this)
			{
				foreach (var di in this)
				{
					if (di.EqualsVidPidUsage(item))
						return (true);
				}

				return (false);
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
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
		/// <see cref="DeviceInfo.ProductId"/>, <see cref="DeviceInfo.Serial"/>, and
		/// <see cref="HidDeviceInfo.UsagePage"/>, <see cref="HidDeviceInfo.UsageId"/> are considered.
		/// </remarks>
		public virtual bool ContainsVidPidSerialUsage(HidDeviceInfo item)
		{
			lock (this)
			{
				foreach (var di in this)
				{
					if (di.EqualsVidPidSerialUsage(item))
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
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only
		/// <see cref="DeviceInfo.VendorId"/>, <see cref="DeviceInfo.ProductId"/> and
		/// <see cref="HidDeviceInfo.UsagePage"/>, <see cref="HidDeviceInfo.UsageId"/> are considered.
		/// </remarks>
		public virtual HidDeviceInfo FindVidPidUsage(HidDeviceInfo item)
		{
			lock (this)
			{
				EqualsVidPidUsage<HidDeviceInfo> predicate = new EqualsVidPidUsage<HidDeviceInfo>(item);
				return (Find(predicate.Match));
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
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
		/// <see cref="DeviceInfo.ProductId"/>, <see cref="DeviceInfo.Serial"/>, and
		/// <see cref="HidDeviceInfo.UsagePage"/>, <see cref="HidDeviceInfo.UsageId"/> are considered.
		/// </remarks>
		public virtual HidDeviceInfo FindVidPidSerialUsage(HidDeviceInfo item)
		{
			lock (this)
			{
				EqualsVidPidSerialUsage<HidDeviceInfo> predicate = new EqualsVidPidSerialUsage<HidDeviceInfo>(item);
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
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only
		/// <see cref="DeviceInfo.VendorId"/>, <see cref="DeviceInfo.ProductId"/> and
		/// <see cref="HidDeviceInfo.UsagePage"/>, <see cref="HidDeviceInfo.UsageId"/> are considered.
		/// </remarks>
		public virtual int FindIndexVidPidUsage(HidDeviceInfo item)
		{
			lock (this)
			{
				EqualsVidPidUsage<HidDeviceInfo> predicate = new EqualsVidPidUsage<HidDeviceInfo>(item);
				return (FindIndex(predicate.Match));
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
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
		/// <see cref="DeviceInfo.ProductId"/>, <see cref="DeviceInfo.Serial"/>, and
		/// <see cref="HidDeviceInfo.UsagePage"/>, <see cref="HidDeviceInfo.UsageId"/> are considered.
		/// </remarks>
		public virtual int FindIndexVidPidSerialUsage(HidDeviceInfo item)
		{
			lock (this)
			{
				EqualsVidPidSerialUsage<HidDeviceInfo> predicate = new EqualsVidPidSerialUsage<HidDeviceInfo>(item);
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
