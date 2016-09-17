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

namespace MKY.IO.Usb
{
	/// <summary>
	/// Abstract base for all HID report containers.
	/// </summary>
	public abstract class HidReportContainer
	{
		private HidDevice device;
		private int maxByteLength;

		/// <summary>
		/// Creates a report container and stores the reference to the device in use as well as
		/// the maximum length of a report.
		/// </summary>
		protected HidReportContainer(HidDevice device, int maxLength)
		{
			this.device = device;
			this.maxByteLength = maxLength;
		}

		/// <summary>
		/// The device associated with this report.
		/// </summary>
		protected HidDevice Device
		{
			get { return (this.device); }
		}

		/// <summary>
		/// The maximum byte length of a report. The length is given by the device capabilities.
		/// </summary>
		public int MaxByteLength
		{
			get { return (this.maxByteLength); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
