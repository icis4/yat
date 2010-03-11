//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

using MKY.Utilities.Globalization;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	public static class StringDescriptor
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private delegate bool GetHidStringDelegate(SafeFileHandle deviceHandle, out string hidString);

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		private static GetHidStringDelegate GetHidStringMethod(StringDescriptorIndex index)
		{
			switch (index)
			{
				case StringDescriptorIndex.Manufacturer: return (Utilities.Win32.Hid.HidD_GetManufacturerString);
				case StringDescriptorIndex.Product:      return (Utilities.Win32.Hid.HidD_GetProductString);
				case StringDescriptorIndex.SerialNumber: return (Utilities.Win32.Hid.HidD_GetSerialNumberString);
				default: throw (new ArgumentOutOfRangeException("index", index, "Invalid string descriptor index"));
			}
		}

		/// <summary></summary>
		public static bool TryGetManufacturerString(string systemPath, out string manufacturer)
		{
			return (TryGetString(systemPath, StringDescriptorIndex.Manufacturer, out manufacturer));
		}

		/// <summary></summary>
		public static bool TryGetProductString(string systemPath, out string product)
		{
			return (TryGetString(systemPath, StringDescriptorIndex.Product, out product));
		}

		/// <summary></summary>
		public static bool TryGetSerialNumberString(string systemPath, out string serialNumber)
		{
			return (TryGetString(systemPath, StringDescriptorIndex.SerialNumber, out serialNumber));
		}

		private static bool TryGetString(string systemPath, StringDescriptorIndex index, out string descriptorString)
		{
			if (Utilities.Win32.Version.IsWindowsVistaOrLater())
				return (TryGetUsbString(systemPath, index, out descriptorString));
			else
				return (TryGetHidString(systemPath, index, out descriptorString));
		}

		private static bool TryGetUsbString(string systemPath, StringDescriptorIndex index, out string usbString)
		{
			SafeFileHandle deviceHandle = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				0,
				Utilities.Win32.FileIO.FILE_SHARE_READ | Utilities.Win32.FileIO.FILE_SHARE_WRITE,
				IntPtr.Zero,
				Utilities.Win32.FileIO.OPEN_EXISTING,
				Utilities.Win32.FileIO.FILE_FLAG_OVERLAPPED | Utilities.Win32.FileIO.FILE_ATTRIBUTE_NORMAL,
				0
				);
			/*SafeFileHandle deviceHandle = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				Utilities.Win32.FileIO.GENERIC_READ | Utilities.Win32.FileIO.GENERIC_WRITE,
				Utilities.Win32.FileIO.FILE_SHARE_READ | Utilities.Win32.FileIO.FILE_SHARE_WRITE,
				IntPtr.Zero,
				Utilities.Win32.FileIO.OPEN_EXISTING,
				Utilities.Win32.FileIO.FILE_FLAG_OVERLAPPED | Utilities.Win32.FileIO.FILE_ATTRIBUTE_NORMAL,
				0
				);*/

			if (!deviceHandle.IsInvalid)
			{
				try
				{
					IntPtr interfaceHandle;
					if (Utilities.Win32.Usb.InitializeHandle(deviceHandle, out interfaceHandle))
					{
						// Retrieve language IDs at index 0
						string s;
						int lengthTransferred;
						if (Utilities.Win32.Usb.GetStringDescriptor(interfaceHandle, (int)StringDescriptorIndex.LanguageIds, 0, out s, out lengthTransferred))
						{
							// Retrieve culture specific strings
							CultureInfo[] l = GetCultureInfoFromLanguageString(s);
							Dictionary<CultureInfo, string> d = GetCultureSpecificStrings(interfaceHandle, index, l);
							CultureInfo ci = XCultureInfo.GetMostAppropriateCultureInfo(d.Keys);
							if ((ci != null) && (d.ContainsKey(ci)))
							{
								usbString = d[ci];
								return (true);
							}
						}
					}
				}
				finally
				{
					deviceHandle.Close();
				}
			}
			usbString = "";
			return (false);
		}

		private static bool TryGetHidString(string systemPath, StringDescriptorIndex index, out string hidString)
		{
			SafeFileHandle deviceHandle = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				0,
				Utilities.Win32.FileIO.FILE_SHARE_READ | Utilities.Win32.FileIO.FILE_SHARE_WRITE,
				IntPtr.Zero,
				Utilities.Win32.FileIO.OPEN_EXISTING,
				0,
				0
				);

			if (!deviceHandle.IsInvalid)
			{
				try
				{
					string s;
					GetHidStringDelegate StringMethod = GetHidStringMethod(index);
					if (StringMethod(deviceHandle, out s)) // GetManufacturerString() or GetProductString() or GetSerialNumberString()
					{
						hidString = s;
						return (true);
					}
				}
				finally
				{
					deviceHandle.Close();
				}
			}
			hidString = "";
			return (false);
		}

		private static CultureInfo[] GetCultureInfoFromLanguageString(string languageString)
		{
			List<CultureInfo> l = new List<CultureInfo>();
			foreach (char c in languageString)
			{
				int culture = (int)c;
				CultureInfo ci = null;
				try
				{
					ci = CultureInfo.GetCultureInfo(culture);
				}
				catch
				{
				}

				if (ci != null)
					l.Add(ci);
			}
			return (l.ToArray());
		}

		private static Dictionary<CultureInfo, string> GetCultureSpecificStrings(IntPtr interfaceHandle, StringDescriptorIndex index, IEnumerable<CultureInfo> cultureInfo)
		{
			Dictionary<CultureInfo, string> d = new Dictionary<CultureInfo, string>();
			foreach (CultureInfo ci in cultureInfo)
			{
				string s;
				int lengthTransferred;
				if (Utilities.Win32.Usb.GetStringDescriptor(interfaceHandle, (int)index, ci.LCID, out s, out lengthTransferred))
					d.Add(ci, s);
			}
			return (d);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
