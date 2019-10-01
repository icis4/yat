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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
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
	public class HidDeviceCollection : DeviceCollection
	{
		private HidUsagePage usagePage = HidUsagePage.Undefined;
		private HidUsageId   usageId   = HidUsageId.Undefined;

		/// <summary></summary>
		public HidDeviceCollection()
			: base(DeviceClass.Hid)
		{
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public HidDeviceCollection(HidUsagePage usagePage)
			: base(DeviceClass.Hid)
		{
			this.usagePage = usagePage;
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public HidDeviceCollection(HidUsagePage usagePage, HidUsageId usageId)
			: base(DeviceClass.Hid)
		{
			this.usagePage = usagePage;
			this.usageId   = usageId;
		}

		/// <summary></summary>
		public HidDeviceCollection(IEnumerable<DeviceInfo> rhs)
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
