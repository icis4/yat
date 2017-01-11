﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
	public class EqualsVidPid
	{
		private DeviceInfo deviceInfo;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public EqualsVidPid(DeviceInfo deviceInfo)
		{
			this.deviceInfo = deviceInfo;
		}

		/// <summary></summary>
		public DeviceInfo DeviceInfo
		{
			get { return (this.deviceInfo); }
			set { this.deviceInfo = value;  }
		}

		/// <summary></summary>
		public Predicate<DeviceInfo> Match
		{
			get { return (IsMatch); }
		}

		private bool IsMatch(DeviceInfo other)
		{
			return (this.deviceInfo.EqualsVidPid(other));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
