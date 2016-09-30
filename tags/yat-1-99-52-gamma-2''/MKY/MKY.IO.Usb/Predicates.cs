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
// MKY Version 1.0.17
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
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
	public class EqualsVidAndPid
	{
		private DeviceInfo deviceInfo;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public EqualsVidAndPid(DeviceInfo deviceInfo)
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
