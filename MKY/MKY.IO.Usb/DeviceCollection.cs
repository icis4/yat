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
// Copyright © 2010-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

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
		public virtual void FillWithAvailableDevices()
		{
			Clear();

			foreach (DeviceInfo di in Device.GetDevicesFromGuid(this.classGuid))
				Add(di);

			Sort();
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
		public virtual bool ContainsVidPid(DeviceInfo item)
		{
			foreach (DeviceInfo di in this)
			{
				if (di.EqualsVidPid(item))
					return (true);
			}

			return (false);
		}

		/// <summary>
		/// Searches for an element that matches the <paramref name="item"/>, and returns the
		/// first occurrence within the entire collection.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the collection. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// The first element that matchesthe <paramref name="item"/>, if found; otherwise, –1.
		/// </returns>
		public virtual DeviceInfo FindVidPid(DeviceInfo item)
		{
			EqualsVidPid predicate = new EqualsVidPid(item);
			return (Find(predicate.Match));
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
		public virtual int FindIndexVidPid(DeviceInfo item)
		{
			EqualsVidPid predicate = new EqualsVidPid(item);
			return (FindIndex(predicate.Match));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
