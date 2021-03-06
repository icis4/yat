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
// MKY Version 1.0.30
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

using System;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	/// <typeparam name="T">The applicable type for the predicate.</typeparam>
	public class EqualsVidPid<T>
		where T : DeviceInfo
	{
		/// <summary></summary>
		public T DeviceInfo { get; set; }

		/// <summary></summary>
		public EqualsVidPid(T deviceInfo)
		{
			DeviceInfo = deviceInfo;
		}

		/// <summary></summary>
		public Predicate<T> Match
		{
			get { return (IsMatch); }
		}

		private bool IsMatch(T other)
		{
			return (DeviceInfo.EqualsVidPid(other));
		}
	}

	/// <summary></summary>
	/// <typeparam name="T">The applicable type for the predicate.</typeparam>
	public class EqualsVidPidSerial<T>
		where T : DeviceInfo
	{
		/// <summary></summary>
		public T DeviceInfo { get; set; }

		/// <summary></summary>
		public EqualsVidPidSerial(T deviceInfo)
		{
			DeviceInfo = deviceInfo;
		}

		/// <summary></summary>
		public Predicate<T> Match
		{
			get { return (IsMatch); }
		}

		private bool IsMatch(T other)
		{
			return (DeviceInfo.EqualsVidPidSerial(other));
		}
	}

	/// <summary></summary>
	/// <typeparam name="T">The applicable type for the predicate.</typeparam>
	public class EqualsVidPidUsage<T>
		where T : HidDeviceInfo
	{
		/// <summary></summary>
		public T DeviceInfo { get; set; }

		/// <summary></summary>
		public EqualsVidPidUsage(T deviceInfo)
		{
			DeviceInfo = deviceInfo;
		}

		/// <summary></summary>
		public Predicate<T> Match
		{
			get { return (IsMatch); }
		}

		private bool IsMatch(T other)
		{
			return (DeviceInfo.EqualsVidPidUsage(other));
		}
	}

	/// <summary></summary>
	/// <typeparam name="T">The applicable type for the predicate.</typeparam>
	public class EqualsVidPidSerialUsage<T>
		where T : HidDeviceInfo
	{
		/// <summary></summary>
		public T DeviceInfo { get; set; }

		/// <summary></summary>
		public EqualsVidPidSerialUsage(T deviceInfo)
		{
			DeviceInfo = deviceInfo;
		}

		/// <summary></summary>
		public Predicate<T> Match
		{
			get { return (IsMatch); }
		}

		private bool IsMatch(T other)
		{
			return (DeviceInfo.EqualsVidPidSerialUsage(other));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
