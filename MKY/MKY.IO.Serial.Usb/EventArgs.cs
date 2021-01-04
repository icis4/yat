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
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Globalization;
using System.Text;

using MKY.IO.Usb;

namespace MKY.IO.Serial.Usb
{
	/// <summary></summary>
	public class SerialDataReceivedEventArgs : DataReceivedEventArgs
	{
		private DeviceInfo deviceInfo;
		private bool useReportId;
		private byte reportId;

		/// <summary></summary>
		public SerialDataReceivedEventArgs(byte data, DeviceInfo deviceInfo, bool useReportId, byte reportId)
			: this(new byte[] { data }, deviceInfo, useReportId, reportId)
		{
		}

		/// <summary></summary>
		public SerialDataReceivedEventArgs(byte[] data, DeviceInfo deviceInfo, bool useReportId, byte reportId)
			: base(data)
		{
			this.deviceInfo  = deviceInfo;
			this.useReportId = useReportId;
			this.reportId    = reportId;
		}

		/// <summary></summary>
		public override string Device
		{
			get
			{
				var sb = new StringBuilder(this.deviceInfo.ToShortString());
				if (this.useReportId)
				{
					sb.Append(" ReportID:");
					sb.Append(this.reportId.ToString(CultureInfo.InvariantCulture));
				}
				return (sb.ToString());
			}
		}
	}

	/// <summary></summary>
	public class SerialDataSentEventArgs : DataSentEventArgs
	{
		private DeviceInfo deviceInfo;
		private bool useReportId;
		private byte reportId;

		/// <summary></summary>
		public SerialDataSentEventArgs(byte[] data, DateTime timeStamp, DeviceInfo deviceInfo, bool useReportId, byte reportId)
			: base(data, timeStamp)
		{
			this.deviceInfo  = deviceInfo;
			this.useReportId = useReportId;
			this.reportId    = reportId;
		}

		/// <summary></summary>
		public override string Device
		{
			get
			{
				var sb = new StringBuilder(this.deviceInfo.ToShortString());
				if (this.useReportId)
				{
					sb.Append(" ReportID:");
					sb.Append(this.reportId.ToString(CultureInfo.InvariantCulture));
				}
				return (sb.ToString());
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
