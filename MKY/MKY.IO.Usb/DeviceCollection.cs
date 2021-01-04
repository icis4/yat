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
// Copyright © 2010-2021 Matthias Kläy.
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
	/// Base of list for containing USB device information.
	/// </summary>
	/// <typeparam name="T">The type of the items in the collection.</typeparam>
	[Serializable]
	public abstract class DeviceCollection<T> : List<T>
		where T : DeviceInfo
	{
		private DeviceClass deviceClass = DeviceClass.Any;
		private Guid        classGuid   = Guid.Empty;

		/// <summary></summary>
		protected DeviceCollection()
		{
		}

		/// <summary></summary>
		protected DeviceCollection(DeviceClass deviceClass)
		{
			this.deviceClass = deviceClass;
			this.classGuid = Device.GetGuidFromDeviceClass(deviceClass);
		}

		/// <summary></summary>
		protected DeviceCollection(IEnumerable<T> rhs)
			: base(rhs)
		{
			var casted = (rhs as DeviceCollection<T>);
			if (casted != null)
			{
				this.deviceClass = casted.deviceClass;
				this.classGuid   = casted.classGuid;
			}
		}

		/// <summary>
		/// Fills list with the available USB devices.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void FillWithAvailableDevices(bool retrieveStringsFromDevice = true)
		{
			lock (this)
			{
				Clear();

				DebugVerboseIndent("Retrieving connected USB devices...");
				foreach (T di in Device.GetDevicesFromGuid(this.classGuid, retrieveStringsFromDevice))
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
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>
		/// and <see cref="DeviceInfo.ProductId"/> are considered.
		/// </remarks>
		public virtual bool ContainsVidPid(T item)
		{
			lock (this)
			{
				foreach (var di in this)
				{
					if (di.EqualsVidPid(item))
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
		/// <see cref="DeviceInfo.ProductId"/> and <see cref="DeviceInfo.Serial"/> are considered.
		/// </remarks>
		public virtual bool ContainsVidPidSerial(T item)
		{
			lock (this)
			{
				foreach (var di in this)
				{
					if (di.EqualsVidPidSerial(item))
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
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>
		/// and <see cref="DeviceInfo.ProductId"/> are considered.
		/// </remarks>
		public virtual T FindVidPid(T item)
		{
			lock (this)
			{
				var predicate = new EqualsVidPid<T>(item);
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
		/// <see cref="DeviceInfo.ProductId"/> and <see cref="DeviceInfo.Serial"/> are considered.
		/// </remarks>
		public virtual T FindVidPidSerial(T item)
		{
			lock (this)
			{
				var predicate = new EqualsVidPidSerial<T>(item);
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
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>
		/// and <see cref="DeviceInfo.ProductId"/> are considered.
		/// </remarks>
		public virtual int FindIndexVidPid(T item)
		{
			lock (this)
			{
				var predicate = new EqualsVidPid<T>(item);
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
		/// <see cref="DeviceInfo.ProductId"/> and <see cref="DeviceInfo.Serial"/> are considered.
		/// </remarks>
		public virtual int FindIndexVidPidSerial(T item)
		{
			lock (this)
			{
				var predicate = new EqualsVidPidSerial<T>(item);
				return (FindIndex(predicate.Match));
			}
		}

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseIndent(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);

			Debug.Indent();
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseUnindent(string message = null)
		{
			Debug.Unindent();

			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);
		}

		#endregion
	}

	/// <summary>
	/// List containing USB device information.
	/// </summary>
	[Serializable]
	public class DeviceCollection : DeviceCollection<DeviceInfo>
	{
		/// <summary></summary>
		public DeviceCollection()
		{
		}

		/// <summary></summary>
		public DeviceCollection(DeviceClass deviceClass)
			: base(deviceClass)
		{
		}

		/// <summary></summary>
		public DeviceCollection(IEnumerable<DeviceInfo> rhs)
			: base(rhs)
		{
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
