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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Ports;

using Microsoft.Win32.SafeHandles;

using MKY.Diagnostics;

namespace MKY.Win32.DotNet
{
	/// <summary>
	/// Implements a workaround to the <see cref="IOException"/> issue in <see cref="SerialPort"/>
	/// based on http://zachsaw.blogspot.ch/2010/07/serialport-ioexception-workaround-in-c.html
	/// by Zach Saw.
	///
	/// Advantages:
	/// Prevents the exception (lookahead) instead of handling it (lookbehind),
	/// the implementation of the workaround is properly encapsulated.
	///
	/// Big disadvantage:
	/// Requires 'Microsoft.Win32.SafeHandles' and several Win32 APIs, thus
	/// introduces platform dependency.
	///
	/// The <see cref="IOException"/> issue can also be solved by actively managing the
	/// <see cref="SerialPort.BaseStream"/>. The implementation in 'MKY.IO.Ports.SerialPort'
	/// uses this approach, in order to not depend upon Win32 specifics.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Patcher", Justification = "What's wrong with 'Patcher'?")]
	public class SerialPortPatcher : DisposableBase
	{
		/// <summary>
		/// Applies the fix to the given serial port.
		/// </summary>
		public static void ApplyTo(string portName)
		{
			using (new SerialPortPatcher(portName))
			{
			}
		}

		private SafeFileHandle handle;

		private SerialPortPatcher(string portName)
		{
			if (string.IsNullOrEmpty(portName) || !StringEx.StartsWithOrdinalIgnoreCase(portName, "COM"))
				throw (new ArgumentException(@"Invalid serial port name, must be ""COM...""!", nameof(portName)));

			SafeFileHandle h = FileIO.NativeMethods.CreateFile
			(
				@"\\.\" + portName,
				FileIO.NativeTypes.Access.GENERIC_READ_WRITE,
				FileIO.NativeTypes.ShareMode.SHARE_NONE,
				IntPtr.Zero,
				FileIO.NativeTypes.CreationDisposition.OPEN_EXISTING,
				FileIO.NativeTypes.AttributesAndFlags.FLAG_OVERLAPPED,
				IntPtr.Zero
			);

			if (h.IsInvalid)
				throw (WinError.LastErrorToIOException());

			try
			{
				var fileType = FileApi.NativeMethods.GetFileType_(h);
				switch (fileType)
				{
					case FileApi.NativeTypes.FileType.FILE_TYPE_CHAR:
					case FileApi.NativeTypes.FileType.FILE_TYPE_UNKNOWN:
					{
						InitializeDCB(h);
						this.handle = h;
						break;
					}

					default:
					{
						throw (new ArgumentException("Invalid serial port handle", nameof(portName)));
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to initialize serial port handle!");

				h.Close();
				this.handle = null;
				throw;
			}
		}

		private static void InitializeDCB(SafeFileHandle handle)
		{
			WinBase.NativeTypes.DCB dcb;
			WinBase.GetCommState(handle, out dcb);
			dcb.Flags.AbortOnError = false;
			WinBase.SetCommState(handle, ref dcb);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			// Dispose of managed resources:
			if (disposing)
			{
				if (this.handle != null) {
					this.handle.Close();
					this.handle = null;
				}
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
