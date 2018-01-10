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
// MKY Version 1.0.23
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	public class EqualsVidPid
	{
		/// <summary></summary>
		public DeviceInfo DeviceInfo { get; set; }

		/// <summary></summary>
		public EqualsVidPid(DeviceInfo deviceInfo)
		{
			DeviceInfo = deviceInfo;
		}

		/// <summary></summary>
		public Predicate<DeviceInfo> Match
		{
			get { return (IsMatch); }
		}

		private bool IsMatch(DeviceInfo other)
		{
			return (DeviceInfo.EqualsVidPid(other));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
